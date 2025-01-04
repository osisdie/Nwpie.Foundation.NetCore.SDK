using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Configuration.SDK.Contracts.Get;
using Nwpie.Foundation.Configuration.SDK.Contracts.Set;
using Nwpie.Foundation.Http.Common.Utilities;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Configuration.SDK.Providers
{
    public class DefaultRemoteConfigClient : CObject, IConfigClient
    {
        /// <summary>
        /// Limited usage: Upsert and GetLatest only
        /// </summary>
        /// <param name="serializer"></param>
        public DefaultRemoteConfigClient(ISerializer serializer)
        {
            m_Serializer = serializer;
            Initialize();
        }

        void Initialize()
        {
            var baseUrl = ServiceContext.ConfigServiceUrl.TrimEndSlash();
            GetUrl = baseUrl.ResolveGetActionUrl();
            SetUrl = baseUrl.ResolveSetActionUrl();
        }

        public virtual async Task<IServiceResponse<T>> GetLatest<T>(string configKey, string apiName = null, string apiKey = null)
            where T : class
        {
            var response = await GetLatest(configKey, apiName, apiKey);
            if (response.Any())
            {
                try
                {
                    return new ServiceResponse<T>(true)
                        .Content(m_Serializer.Deserialize<T>(response.Data));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return new ServiceResponse<T>()
                        .Error(StatusCodeEnum.SerializationError, ex.ToString());
                }
            }

            return new ServiceResponse<T>(response as ServiceResponse);
        }

        public virtual async Task<IServiceResponse<string>> GetLatest(string configKey, string apiName = null, string apiKey = null)
        {
            var response = await GetLatest(configs: new List<ConfigItem> { new ConfigItem(configKey) },
                apiName: apiName,
                apiKey: apiKey
            );

            if (response.IsSuccess && true == response.Data?.ContainsKeyIgnoreCase(configKey))
            {
                return new ServiceResponse<string>(true, response.Data.First(o => string.Equals(o.Key, configKey, StringComparison.OrdinalIgnoreCase)).Value);
            }

            return new ServiceResponse<string>(response as IServiceResponse);
        }

        public virtual async Task<IServiceResponse<Dictionary<string, string>>> GetLatest(List<ConfigItem> configs, string apiName = null, string apiKey = null)
        {
            var result = new ServiceResponse<Dictionary<string, string>>();
            try
            {
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { CommonConst.ApiName, apiName ?? ServiceContext.ApiName },
                    { CommonConst.ApiKey, apiKey ?? ServiceContext.ApiKey }
                };

                configs.ForEach(o => o.Version = o.Version ?? ConfigConst.LatestVersion);
                var json = m_Serializer.Serialize(new
                {
                    Data = new
                    {
                        ConfigKeys = configs
                    }
                });

                await RetryHelper.RetryOnException(DefaultRetries, TimeSpan.FromSeconds(DefaultDelayRetrySecs), async () =>
                {
                    var errMsg = string.Empty;
                    var response = await ApiUtils.HttpPost(GetUrl,
                        json,
                        headers,
                        DefaultTimeoutSecs
                    );

                    if (response.Any())
                    {
                        var getStatus = m_Serializer.Deserialize<ConfigGet_Response>(response.Data);

                        errMsg = getStatus?.ErrMsg;
                        if (getStatus.IsSuccess && getStatus.Data?.RawData?.Count() > 0)
                        {
                            result.Success().Content(getStatus.Data.RawData);
                            return;
                        }

                        Logger.LogWarning(response.ErrMsg ?? errMsg ?? $"Failed to get config from {GetUrl}. ");

                        if (getStatus.IsSuccess ||
                            (int)StatusCodeEnum.EmptyData == getStatus.Code ||
                            getStatus.Code > (int)StatusCodeEnum.ErrorCode_Max)
                        {
                            result.Success(StatusCodeEnum.EmptyData);
                            return;
                        }
                    }

                    result.Error(StatusCodeEnum.Error, response.ErrMsg ?? errMsg ?? $"Failed to get config from {GetUrl}. ");
                    throw new Exception(result.ErrMsg);
                });

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.GetBaseFirstExceptionString());
                return new ServiceResponse<Dictionary<string, string>>()
                    .Error(StatusCodeEnum.Exception, ex.GetBaseFirstExceptionString());
            }
        }

        public virtual async Task<IServiceResponse<bool>> Upsert(string configKey, string config, bool encrypt, string apiName = null, string apiKey = null)
        {
            var result = new ServiceResponse<bool>();
            try
            {
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { CommonConst.ApiName, apiName ?? ServiceContext.ApiName },
                    { CommonConst.ApiKey, apiKey ?? ServiceContext.ApiKey }
                };

                var json = m_Serializer.Serialize(new
                {
                    Data = new SetConfig_RequestModel()
                    {
                        //VersionDisplay = ConfigConst.LatestVersion,
                        ConfigKey = configKey,
                        RawData = config,
                        NeedEncrypt = encrypt
                    }
                });

                await RetryHelper.RetryOnException(DefaultRetries,
                    TimeSpan.FromSeconds(DefaultDelayRetrySecs),
                    async () =>
                    {
                        var response = await ApiUtils.HttpPost(SetUrl,
                            json,
                            headers,
                            DefaultTimeoutSecs
                        );

                        if (response.Any())
                        {
                            var setStatus = m_Serializer
                                .Deserialize<ConfigSet_Response>(response.Data);

                            result.Success().Content(setStatus.IsSuccess);
                            return;
                        }

                        result.Error(StatusCodeEnum.Error, response.ErrMsg ?? response.Msg ?? $"Failed to set latest config from {SetUrl}. ");
                        throw new Exception(result.ErrMsg);
                    }
                );

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return new ServiceResponse<bool>()
                    .Error(StatusCodeEnum.Exception, ex.ToString());
            }
        }

        public void Dispose() { }

        public static int GlobalTimeoutSecs = ConfigConst.DefaultHttpTimeout * 2;
        public static int GlobalRetries = ConfigConst.DefaultRemoteConfigRetry;
        public static int GlobalDelayRetrySecs = ConfigConst.DefaultDelayRetrySecs;

        public string GetUrl { get; set; }
        public string SetUrl { get; set; }

        public int DefaultTimeoutSecs { get; set; } = GlobalTimeoutSecs;
        public int DefaultRetries { get; set; } = GlobalRetries;
        public int DefaultDelayRetrySecs { get; set; } = GlobalDelayRetrySecs;

        public readonly ISerializer m_Serializer;
    }
}
