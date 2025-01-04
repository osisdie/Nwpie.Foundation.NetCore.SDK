using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using Microsoft.Extensions.Configuration;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Services
{
    /// <summary>
    /// db.version,app.version,app.env,s3.test
    /// cache.local.test,cache.local.flush
    /// cache.redis.test,cache.redis.flush
    /// </summary>
    public class HealthCheckEvent : CObject, IHealthCheckEvent
    {
        public HealthCheckEvent(HlckEcho_DomainService service, HlckEcho_ParamModel param, HlckEcho_ResponseModel result)
        {
            result = result ?? new HlckEcho_ResponseModel();
            DomainService = service;
            ParamDto = param;
            ResultDto = result;
            result.ExtensionMap = result.ExtensionMap
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public async Task Execute()
        {
            var commands = ParamDto?.RequestString
                ?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                ?? new string[] { };

            foreach (var command in commands)
            {
                switch (command)
                {
                    case "exception": throw new Exception("manual exception");
                    case "auth.get": await OnAuthGet(); break;
                    case "auth.expire": await OnAuthExpire(); break;
                    case "db.version": await OnDatabaseVersion(); break;
                    case "app.version": await OnAppVersion(); break;
                    case "app.env": await OnAppEnv(); break;
                    case "app.config": await OnAppConfig(); break;
                    case "app.mapper": await OnAppMapper(); break;
                    case "s3.test": await OnS3(); break;
                    case "cache.provider": await OnCacheProvider(); break;
                    case "cache.local.test": await OnLocalCache(); break;
                    case "cache.redis.test": await OnRedisCache(); break;
                    case "cache.local.flush": await OnLocalCacheFlush(); break;
                    case "cache.redis.flush": await OnRedisCacheFlush(); break;
                    case "notify.email.test": await OnEmail(); break;
                    case "notify.line.test": await OnLine(); break;
                    case "notify.slack.test": await OnSlack(); break;
                    default:
                        break;
                };
            };

            if (m_ExecutedCount > 0)
            {
                return;
            }

            // Auth
            var authAsk = commands
                .Where(o => o.StartsWith("auth.@"))
                .FirstOrDefault();
            if (authAsk.HasValue())
            {
                await OnAuthAsk(authAsk.Replace("auth.@", "")).ConfigureAwait(false);
            }
        }

        public virtual async Task OnAuthExpire()
        {
            var authService = ComponentMgr.Instance.GetDefaultJwtTokenService();
            if (null != authService)
            {
                var model = await authService.GenerateAdminTokenModel<TokenDataModel>();
                if (null != model?.AccountId)
                {
                    model.Exp = DateTime.UtcNow.AddMinutes(-1);
                }

                var bearerToken = await authService.Encode(model);
                ResultDto.ExtensionMap["auth.expire.provider"] = authService?.GetType()?.Name;
                ResultDto.ExtensionMap["auth.expire"] = bearerToken;
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAuthGet()
        {
            var authService = ComponentMgr.Instance.GetDefaultJwtTokenService();
            var model = await authService?.GenerateAdminTokenModel<TokenDataModel>();
            var bearerToken = await authService?.Encode(model);

            ResultDto.ExtensionMap["auth.get.provider"] = authService?.GetType()?.Name;
            ResultDto.ExtensionMap["auth.get"] = bearerToken;

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAuthAsk(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var attr = new EmailAddressAttribute();
            var isValid = attr.IsValid(email);
            if (false == isValid)
            {
                return;
            }

            var authService = ComponentMgr.Instance.TryResolve<IJwtAuthService>();
            if (null != authService)
            {
                var account = await TokenProfileMgr.Instance.GetProfileByEmailAsync<AccountProfileBase>(email);
                if (null != account)
                {
                    var model = new TokenDataModel()
                    {
                        //AppId = ""
                        AccountId = account.AccountId,
                        Name = account.Name,
                        IP = null,
                        Mob = true,
                        Kind = (byte)TokenKindEnum.AccessToken,
                        LV = (byte)TokenLevelEnum.EndUser,
                        Iss = authService.GetType().Name,
                        Exp = DateTime.UtcNow.AddMinutes(1440),
                        Upt = DateTime.UtcNow,
                        Iat = DateTime.UtcNow
                    };

                    var bearerToken = await authService.Encode(model);
                    ResultDto.ExtensionMap["auth.ask.provider"] = authService.GetType().Name;
                    ResultDto.ExtensionMap["auth.ask"] = $"{bearerToken}";
                }
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnCommandExecuted()
        {
            await Task.CompletedTask;
            Interlocked.Increment(ref m_ExecutedCount);
        }

        public virtual async Task OnDatabaseVersion()
        {
            var repo = ComponentMgr.Instance.TryResolve<IHlckEcho_Repository>();
            if (null != repo)
            {
                var version = await repo.ExecuteQueryVersion(ParamDto);
                ResultDto.ExtensionMap["db.version"] = version;
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAppVersion()
        {
            ResultDto.ExtensionMap["app.version"] = ServiceContext.Version;
            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAppEnv()
        {
            ResultDto.ExtensionMap["app.env"] = ServiceContext.ApiName;
            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAppConfig()
        {
            foreach (var config in ServiceContext.Config.RemoteConfigKeys)
            {
                ResultDto.ExtensionMap[config] = ServiceContext.Configuration
                    .GetValue<string>(config)
                    .HasValue()
                    .ToString();
            }
            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnAppMapper()
        {
            try
            {
                var entity = new HlckEcho_Entity()
                {
                    words = ParamDto?.RequestString
                };

                var converted = HlckEcho_Converter.ConvertToResponseModel(entity);
                ResultDto.ExtensionMap["app.mapper"] = converted?.ResponseString;
            }
            catch (Exception ex)
            {
                ResultDto.ExtensionMap["app.mapper"] = ex.ToString();
            }

            await OnCommandExecuted().ConfigureAwait(false);

        }

        public virtual async Task OnS3()
        {
            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnCacheProvider()
        {
            var cache = ComponentMgr.Instance.GetDefaultCacheFromConfig();
            ResultDto.ExtensionMap["cache.provider"] = cache?.GetType()?.Name;

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnLocalCache()
        {
            ICache cache = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            if (null != cache)
            {
                var cacheKey = $"{ServiceContext.ServiceName}-cache-{cache.GetType().Name}-healthcheck";
                var setResult = await cache.SetAsync(cacheKey, ResultDto.ResponseString, 10);
                ResultDto.ExtensionMap["cache.local.provider"] = cache?.GetType()?.Name;
                ResultDto.ExtensionMap["cache.local.set"] = $"{setResult?.IsSuccess}.{setResult?.Data}";

                if (false == string.IsNullOrWhiteSpace(setResult?.ErrMsg))
                {
                    ResultDto.ExtensionMap["cache.local.set.errMsg"] = setResult.ErrMsg;
                }

                if (setResult.Any())
                {
                    var getResult = await cache.GetAsync<string>(cacheKey);
                    ResultDto.ExtensionMap["cache.local.get"] = $"{getResult?.IsSuccess}.{getResult?.Data}";
                }
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnRedisCache()
        {
            ICache cache = ComponentMgr.Instance.GetDefaultRedisCache();
            if (null == cache)
            {
                var cacheKey = $"{ServiceContext.ServiceName}-cache-{cache.GetType().Name}-healthcheck";
                var setResult = await cache.SetAsync(cacheKey, ResultDto.ResponseString, 10);
                ResultDto.ExtensionMap["cache.redis.provider"] = cache?.GetType()?.Name;
                ResultDto.ExtensionMap["cache.redis.set"] = $"{setResult?.IsSuccess}.{setResult?.Data}";

                if (false == string.IsNullOrWhiteSpace(setResult?.ErrMsg))
                {
                    ResultDto.ExtensionMap["cache.redis.set.errMsg"] = setResult.ErrMsg;
                }

                var getResult = await cache.GetAsync<string>(cacheKey);
                ResultDto.ExtensionMap["cache.redis.get"] = $"{getResult?.IsSuccess}.{getResult?.Data}";
                if (false == string.IsNullOrWhiteSpace(getResult?.ErrMsg))
                {
                    ResultDto.ExtensionMap["cache.redis.get.errMsg"] = getResult.ErrMsg;
                }
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnLocalCacheFlush()
        {
            var cache = ComponentMgr.Instance.TryResolve<ILocalCache>();
            if (null != cache)
            {
                var result = await cache.RemovePatternAsync("*");
                ResultDto.ExtensionMap["cache.local.flush"] = $"{result?.Count()}.{string.Join(",", result?.Select(o => o.Key))}";
            }
            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnRedisCacheFlush()
        {
            var cache = ComponentMgr.Instance.TryResolve<IRedisCache>();
            if (null != cache)
            {
                var result = await cache.RemovePatternAsync("*");
                ResultDto.ExtensionMap["cache.redis.flush"] = $"{result?.Count()}.{string.Join(",", result?.Select(o => o.Key))}";
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnEmail()
        {
            var client = ComponentMgr.Instance.GetDefaultNotificationSQSClient();
            var isReady = false;
            if (null != client)
            {
                try
                {
                    isReady = await client.IsReady();
                }
                catch (Exception ex)
                {
                    ResultDto.ExtensionMap["notify.email.test"] = ex.ToString();
                }

                if (isReady)
                {
                    var res = await client.SendAsync(new NotifySend_RequestModel()
                    {
                        Kind = (int)NotifyChannelEnum.Email,
                        Title = $"{ServiceContext.ApiName}-email notifcaiton healthcheck",
                        ToList = "default",
                        Message = "Awesome !"
                    });

                    ResultDto.ExtensionMap["notify.email.test"] = $"{res?.IsSuccess}.{res.MsgId}";
                }
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnLine()
        {
            var client = ComponentMgr.Instance.GetDefaultNotificationSQSClient();
            var isReady = false;
            if (null != client)
            {
                try
                {
                    isReady = await client.IsReady();
                }
                catch (Exception ex)
                {
                    ResultDto.ExtensionMap["notify.line.test"] = ex.ToString();
                }

                if (isReady)
                {
                    var res = await client.SendAsync(new NotifySend_RequestModel()
                    {
                        Kind = (int)NotifyChannelEnum.Line,
                        Title = $"{ServiceContext.ApiName}-line-notifcaiton-healthcheck",
                        ToList = "default",
                        Message = "Awesome !"
                    });

                    ResultDto.ExtensionMap["notify.line.test"] = $"{res?.IsSuccess}.{res?.MsgId}";
                }
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public virtual async Task OnSlack()
        {
            var client = ComponentMgr.Instance.GetDefaultNotificationSQSClient();
            var isReady = false;
            if (null != client)
            {
                try
                {
                    isReady = await client.IsReady();
                }
                catch (Exception ex)
                {
                    ResultDto.ExtensionMap["notify.slack.test"] = ex.ToString();
                }

                var res = await client.SendAsync(new NotifySend_RequestModel()
                {
                    Kind = (int)NotifyChannelEnum.Line,
                    Title = $"{ServiceContext.ApiName}-slack-notifcaiton-healthcheck",
                    ToList = "default",
                    Message = "Awesome !"
                });

                ResultDto.ExtensionMap["notify.slack.test"] = $"{res?.IsSuccess}.{res?.MsgId}";
            }

            await OnCommandExecuted().ConfigureAwait(false);
        }

        public HlckEcho_DomainService DomainService { get; private set; }
        public HlckEcho_ParamModel ParamDto { get; private set; }
        public HlckEcho_ResponseModel ResultDto { get; private set; }

        protected int m_ExecutedCount = 0;
    }
}
