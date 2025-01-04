using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Auth.Contract.Account.GetProfile;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Token.ValidateApiKey;
using Nwpie.Foundation.Auth.Contract.Token.ValidateToken;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.Auth.SDK
{
    public class TokenProfileMgr : SingleCObject<TokenProfileMgr>
    {
        protected override void InitialInConstructor()
        {
            m_AuthServiceUrl = DefaultAuthServiceUrl;
            m_FailbackScore = FailbackScoreInfo.CreateNew;
#if DEBUG
            m_Profiles.TryAdd("fake_admin_id", new AccountProfileBase
            {
                AccountId = "fake_admin_id",
                Name = "Fake Admin"
            });

            m_Profiles.TryAdd("fake_app_id.debug", new AccountProfileBase
            {
                AccountId = "fake_app_id.debug",
                Name = "Fake App"
            });

            m_Profiles.TryAdd("fake_api_key.debug", new AccountProfileBase
            {
                AccountId = "fake_api_key.debug",
                Name = "fake_api_name.debug"
            });

            m_Profiles.TryAdd("fake_user_id", new AccountProfileBase
            {
                AccountId = "fake_user_id",
                Name = "Fake User"
            });
#endif
            _ = Task.Delay(DefaultDelayStart).ContinueWith(t =>
            {
                StartRepeatPurgeQueueTimer();
            });
        }

        private void StartRepeatPurgeQueueTimer()
        {
            m_Timer = new Timer(TimerCallback,
                null,
                m_FlushInterval,
                Timeout.Infinite
            );
        }

        private void TimerCallback(object state)
        {
            if (m_FailbackScore.IsExceedLimit())
            {
                return;
            }

            try
            {
                var expired = m_Profiles
                    .Where(o => DateTime.UtcNow >= o.Value._ts)
                    .Select(o => o.Key).ToList();
                if (expired?.Count() > 0)
                {
                    expired.ForEach(o => RemoveProfile<AccountProfileBase>(o, out _));
                }

                m_FailbackScore.Score(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                m_FailbackScore.Fail();
            }

            RestartTimer();
        }

        private void RestartTimer()
        {
            m_Timer?.Change(m_FlushInterval, Timeout.Infinite);
        }

        public bool IsExists<T>(string key) where T : AccountProfileBase =>
            m_Profiles.ContainsKey(key);

        private async Task<TResponse> RequestByLocationAsync<TResponse>(object request, string senderApiName = null, string senderApiKey = null)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader();
            if (senderApiKey.HasValue())
            {
                headers[CommonConst.ApiKey] = senderApiKey;
            }

            if (senderApiName.HasValue())
            {
                headers[CommonConst.ApiName] = senderApiName;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(m_AuthServiceUrl))
                {
                    var locationResp = await request.InvokeAsyncByServiceName<TResponse>(
                        serviceName: AuthServiceConfig.ServiceName,
                        headers: headers
                    );

                    return locationResp;
                }

                var authResp = await request.InvokeAsyncByBaseUrl<TResponse>(
                    baseUrl: m_AuthServiceUrl,
                    headers: headers
                );

                return authResp;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return default;
        }

        public async Task<T> GetApiKeyProfileAsync<T>(HttpRequest request, AuthExactFlagEnum flags)
            where T : AccountProfileBase
        {
            var apiKey = await TokenUtils.ExtractApiKey(request, flags);
            if (apiKey.HasValue())
            {
                return await GetApiKeyProfileAsync<T>(apiKey);
            }

            return default;
        }

        public async Task<T> ValidateAndQueryTokenAsync<T>(string token, ITokenDataModel meta, string senderApiName = null, string senderApiKey = null)
            where T : AccountProfileBase
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return default;
            }

            var exists = GetProfile<T>(token);
            if (null != exists)
            {
                return exists;
            }

            var request = new TkValidateToken_Request
            {
                Data = new TkValidateToken_RequestModel
                {
                    Token = token,
                    IP = meta?.IP,
                    UA = meta?.UA,
                    Mac = meta?.Mac
                }
            };

            var response = await RequestByLocationAsync<TkValidateToken_Response>(request,
                senderApiName: senderApiName,
                senderApiKey: senderApiKey
            );
            if (null == response?.Data?.Profile)
            {
                throw new Exception(response?.ErrMsg ?? response?.Msg ?? StatusCodeEnum.InvalidAccessToken.GetDisplayName());
            }

            response.Data.Profile._ts = DateTime.UtcNow + MaxExpireTimeSpan;
            var profile = new AccountProfileBase().PopulateWith(response.Data.Profile);
            if (profile is T)
            {
                _ = TryAddProfile<T>(token, profile as T);
            }

            return GetProfile<T>(token);
        }

        public async Task<T> GetApiKeyProfileAsync<T>(string apiKey, string senderApiName = null, string senderApiKey = null)
            where T : AccountProfileBase
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return default;
            }

            var exists = GetProfile<T>(apiKey);
            if (null != exists)
            {
                return exists;
            }

            try
            {
                var request = new TkValidateApiKey_Request
                {
                    Data = new TkValidateApiKey_RequestModel
                    {
                        ApiKey = apiKey
                    }
                };

                var response = await RequestByLocationAsync<TkValidateApiKey_Response>(request,
                    senderApiName: senderApiName,
                    senderApiKey: senderApiKey
                );
                if (null != response?.Data?.Profile)
                {
                    response.Data.Profile._ts = DateTime.UtcNow + MaxExpireTimeSpan;
                    var profile = new AccountProfileBase().PopulateWith(response.Data.Profile);
                    if (profile is T)
                    {
                        _ = TryAddProfile<T>(apiKey, profile as T);
                    }

                    return GetProfile<T>(apiKey);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return default;
        }

        // email or app_name
        public async Task<T> GetProfileByEmailAsync<T>(string email, string senderApiName = null, string senderApiKey = null)
            where T : AccountProfileBase
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return default;
            }

            var exists = GetProfile<T>(email);
            if (null != exists)
            {
                return exists;
            }

            try
            {
                var request = new AcctGetProfile_Request
                {
                    Data = new AcctGetProfile_RequestModel
                    {
                        Email = email
                    }
                };

                var response = await RequestByLocationAsync<AcctGetProfile_Response>(request,
                    senderApiName: senderApiName,
                    senderApiKey: senderApiKey
                );
                if (null != response?.Data)
                {
                    response.Data._ts = DateTime.UtcNow + MaxExpireTimeSpan;
                    if (response.Data is T)
                    {
                        _ = TryAddProfile<T>(email, response.Data as T);
                    }

                    return GetProfile<T>(email);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return default;
        }

        public async Task<T> GetProfileByAccountIdAsync<T>(string accountId, string senderApiName = null, string senderApiKey = null)
            where T : AccountProfileBase
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return default;
            }

            var exists = GetProfile<T>(accountId);
            if (null != exists)
            {
                return exists;
            }

            try
            {
                var request = new AcctGetProfile_Request
                {
                    Data = new AcctGetProfile_RequestModel
                    {
                        AccountId = accountId
                    }
                };

                var response = await RequestByLocationAsync<AcctGetProfile_Response>(request,
                    senderApiName: senderApiName,
                    senderApiKey: senderApiKey
                );
                if (null != response?.Data)
                {
                    response.Data._ts = DateTime.UtcNow + MaxExpireTimeSpan;
                    if (response.Data is T)
                    {
                        _ = TryAddProfile<T>(accountId, response.Data as T);
                    }

                    return GetProfile<T>(accountId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return default;
        }

        public bool TryAddProfile<T>(string key, T profile)
            where T : AccountProfileBase
        {
            if (null == profile)
            {
                return false;
            }

            return m_Profiles.TryAdd(key, profile);
        }

        public T GetProfile<T>(string key)
           where T : AccountProfileBase
        {
            if (m_Profiles.TryGetValue(key, out var profile))
            {
                return (T)profile;
            }

            return default;
        }

        public bool UpsertProfile<T>(string key, T profile)
           where T : AccountProfileBase
        {
            if (null == profile)
            {
                return false;
            }

            profile._ts = DateTime.UtcNow + MaxExpireTimeSpan;
            var exists = GetProfile<T>(key);
            if (null != exists)
            {
                return m_Profiles.TryUpdate(key, profile, exists);
            }

            return m_Profiles.TryAdd(key, profile);
        }

        public bool RemoveProfile<T>(string key, out T profile)
           where T : AccountProfileBase
        {
            profile = default;
            if (m_Profiles.TryRemove(key, out var p))
            {
                profile = p as T;
                return true;
            }

            return false;
        }

        public void FlushAll()
        {
            m_Profiles?.Clear();
        }


        public override void Dispose()
        {

        }

        public const int DefaultDelayStart = 5 * 1000; // 5 secs
        public const int DefaultFlushInterval = 5 * 60 * 1000; // 5 mins
        public const int DefaultMaxQueueCapacity = 10000;
        public const int DefaultPersistSeconds = 2 * 60 * 60; // 2 hours
        public readonly string DefaultAuthServiceUrl = ServiceContext.AuthServiceUrl;

        public string AuthServiceUrl
        {
            get => m_AuthServiceUrl;
            set => m_AuthServiceUrl = value;
        }

        private string m_AuthServiceUrl;
        private readonly int m_MaxQueueLength = DefaultMaxQueueCapacity;
        private readonly int m_FlushInterval = DefaultFlushInterval; // 5 mins
        private Timer m_Timer;
        private IFailbackScore m_FailbackScore;

        private readonly TimeSpan MaxExpireTimeSpan = TimeSpan.FromSeconds(DefaultPersistSeconds);
        private readonly ConcurrentDictionary<string, AccountProfileBase> m_Profiles = new ConcurrentDictionary<string, AccountProfileBase>(StringComparer.OrdinalIgnoreCase);
    }
}
