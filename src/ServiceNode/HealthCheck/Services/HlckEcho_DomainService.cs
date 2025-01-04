using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Services
{
    public class HlckEcho_DomainService :
        DomainService,
        IHlckEcho_DomainService
    {
        public async Task<HlckEcho_ResponseModel> Execute(HlckEcho_ParamModel param)
        {
            Validate(param);

            var cacheKey = CacheUtils.CacheKeyWithFuncAndParam(CryptoUtils.GetMD5String(param.Convert4Cache<HlckEcho_RequestModel>().ToJson().ToMD5()));
            var result = new HlckEcho_ResponseModel();
            if (null != GetCache())
            {
                var res = await GetCache().GetAsync<HlckEcho_ResponseModel>(cacheKey).ConfigureAwait(false);
                if (res.Any())
                {
                    IsCacheHit = true;
                    return res.Data;
                }
            }

            // form repo
            var data = await GetRepository<IHlckEcho_Repository>()
                .ExecuteQuery(param);
            if (null != data)
            {
                result.ResponseString = data.words;
                await new HealthCheckEvent(this, param, result).Execute();

                if (null != result.ResponseString)
                {
                    FillCacheInfo(result, cacheKey);

                    if (null != GetCache())
                    {
                        _ = await GetCache().SetAsync(cacheKey, result, ConfigConst.DefaultCacheSecs);
                    }
                }

                return result;
            }

            return result;
        }

        public bool Validate(HlckEcho_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
