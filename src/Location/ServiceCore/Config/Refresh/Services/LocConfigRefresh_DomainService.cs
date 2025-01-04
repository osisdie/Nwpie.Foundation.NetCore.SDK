using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.MessageQueue.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config;
using Nwpie.Foundation.Location.Contract.Config.Refresh;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Interfaces;
using Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Models;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Services
{
    public class LocConfigRefresh_DomainService :
        DomainService,
        ILocConfigRefresh_DomainService
    {
        public async Task<LocConfigRefresh_ResponseModel> Execute(LocConfigRefresh_ParamModel param)
        {
            Validate(param);

            var apiName = await GetApiNameAsync();
            var apiKey = await TokenUtils.ExtractApiKey(CurrentRequest,
                AuthExactFlagEnum.ApiKeyHeader |
                AuthExactFlagEnum.ApiKeyInToken
            );
            var configValues = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            var response = new LocConfigRefresh_ResponseModel
            {
                ExtensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };

            var extendedDictionary = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (true == param?.Prefix?.Trim()?.Length >= ConfigConst.PartialWordSearchMinLen)
            {
                var cachePattern = param.Prefix.Trim().AttachPrefix(apiName)
                    + ConfigConst.CacheKeyScanKeywordSuffix;

                if (null != GetCache())
                {
                    var removed = await GetCache().RemovePatternAsync(cachePattern).ConfigureAwait(false);
                    if (removed.Any())
                    {
                        Parallel.ForEach(removed, item =>
                        {
                            extendedDictionary.TryAdd(item.Key, "removed");
                            var configKey = item.Key.DetachPrefix(apiName);
                            configValues.TryAdd(configKey,
                                true == item.Value?.IsSuccess
                            );
                        });
                    }
                }
            }
            else
            {
                Parallel.ForEach(param.ConfigKeys.Distinct(), configKey =>
                {
                    var cacheKey = configKey.AttachPrefix(apiName);
                    if (null != GetCache())
                    {
                        var removed = GetCache().RemoveAsync(cacheKey).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (removed.Any())
                        {
                            extendedDictionary.TryAdd(cacheKey, "removed");
                            configValues.TryAdd(configKey,
                                true == removed?.IsSuccess
                            );
                        }
                    }
                });
            }

            if (extendedDictionary?.Count > 0)
            {
                if (ServiceContext.IsDebugOrDevelopment())
                {
                    response.ExtensionMap = extendedDictionary
                        .ToDictionary(o => o.Key, o => o.Value);
                }
            }

            if (configValues?.Count > 0)
            {
                response.RawData = configValues
                    .ToDictionary(o => o.Key, o => o.Value);

                _ = BroadCastRefersh(new CommandModel
                {
                    Topic = LocationConst.HttpPathToConfigContractRequest_Refresh,
                    Name = LocationConst.HttpPathToConfigContractRequest_Refresh,
                    Data = configValues.Where(o => true == o.Value)
                        .Select(o => o.Key)
                        .ToJson()
                });
            }

            if (true == param.PullLatest)
            {
                _ = PullLastestConfig(configValues
                    .Where(o => true == o.Value)
                    .Select(o => o.Key)
                    .ToList());
            }

            return response;
        }

        public async Task PullLastestConfig(IEnumerable<string> configKeys)
        {
            if (true != configKeys?.Count() > 0)
            {
                return;
            }

            var apiName = await GetApiNameAsync();
            var apiKey = await TokenUtils.ExtractApiKey(CurrentRequest,
                AuthExactFlagEnum.ApiKeyHeader |
                AuthExactFlagEnum.ApiKeyInToken
            );

            foreach (var configKey in configKeys)
            {
                try
                {
                    var configValue = (
                        await m_ConfigClient.GetLatest(configKey: configKey,
                            apiName: apiName,
                            apiKey: apiKey
                        ))?.Data;

                    if (configValue.HasValue())
                    {
                        Logger.LogInformation(string.Format(
                            SysMsgConst.SuccessRetrieveConfigValue,
                            configKey)
                        );

                        if (null != GetCache())
                        {
                            var cacheKey = configKey.AttachPrefix(apiName);
                            _ = await GetCache().SetAsync(cacheKey, configValue, LocationConst.DefaultConfigCacheSecs).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        Logger.LogError($"Failed to get config(key ={configKey}) from configServer. ");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to get config(key ={configKey}) from configServer. ex={ex}");
                }
            }
        }

        public bool Validate(LocConfigRefresh_ParamModel param)
        {
            return param.ValidateAndThrow();
        }
    }
}
