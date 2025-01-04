using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Location.Contract.Config.Get;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.Config.Get.Interfaces;
using Nwpie.Foundation.Location.ServiceCore.Config.Get.Models;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Get
{
    public class LocConfigGet_DomainService :
        DomainService,
        ILocConfigGet_DomainService
    {
        public async Task<LocConfigGet_ResponseModel> Execute(LocConfigGet_ParamModel param)
        {
            Validate(param);

            var apiName = await GetApiNameAsync();
            var apiKey = await TokenUtils.ExtractApiKey(CurrentRequest,
                AuthExactFlagEnum.ApiKeyHeader |
                AuthExactFlagEnum.ApiKeyInToken
            );

            var response = new LocConfigGet_ResponseModel();
            var configValues = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var extendedDictionary = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in param.ConfigKeys)
            {
                var cacheKey = item.ConfigKey.AttachPrefix(apiName);
                if (null != GetCache())
                {
                    var cached = await GetCache().GetAsync<string>(cacheKey).ConfigureAwait(false);
                    if (cached.Any())
                    {
                        configValues.TryAdd(item.ConfigKey, cached.Data);
                        extendedDictionary.TryAdd(item.ConfigKey, "cached");
                        continue;
                    }
                }

                var configValue = await m_ConfigClient
                    .GetLatest(configKey: item.ConfigKey,
                        apiName: apiName,
                        apiKey: apiKey
                    );

                if (configValue.Any())
                {
                    configValues.TryAdd(item.ConfigKey, configValue.Data);

                    if (null != GetCache())
                    {
                        _ = await GetCache().SetAsync(cacheKey, configValue.Data, LocationConst.DefaultConfigCacheSecs);
                    }
                }
            };

            response.ExtensionMap = extendedDictionary
                .ToDictionary(o => o.Key, o => o.Value);

            if (configValues.Count > 0)
            {
                response.RawData = configValues
                    .ToDictionary(o => o.Key, o => o.Value);
            }

            return response;
        }

        public bool Validate(LocConfigGet_ParamModel param)
        {
            return param.ValidateAndThrow();
        }
    }
}
