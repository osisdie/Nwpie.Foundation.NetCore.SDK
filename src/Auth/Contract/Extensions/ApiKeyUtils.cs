using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Auth.Contract.Extensions
{
    public static class ApiKeyUtils
    {
        public static IDictionary<string, string> AddApiKeyHeader(this IDictionary<string, string> o)
        {
            if (null == o)
            {
                return o;
            }

            if (SdkRuntime.ApiKey.HasValue())
            {
                o[CommonConst.ApiKey] = SdkRuntime.ApiKey;

                if (SdkRuntime.ApiName.HasValue())
                {
                    o[CommonConst.ApiName] = SdkRuntime.ApiName;
                }
            }
            else if (SdkRuntime.ApiBearToken.HasValue())
            {
                o[CommonConst.AuthHeaderName] = SdkRuntime.ApiBearToken.AttachBearer();
            }

            return o;
        }

        [Obsolete]
        public static bool IsValidApiNaming(string apiName) =>
            AuthServiceConfig.MatchApiNamePattern.IsMatch(apiName);
    }
}
