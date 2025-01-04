using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.Common.Attributes;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.Contract.Configserver.SetDirectly;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;
using Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Interfaces;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.SetDirectly
{
    [CustomApiKeyFilter]
    public class KvsSetDirectly_Service : HttpRequestServiceBase
    {
        public KvsSetDirectly_Service(IKvsSetDirectly_DomainService service)
        {
            m_Service = service;
        }

        public async Task<object> Any(KvsSetDirectly_Request request)
        {
            m_Service.Headers.CopyFrom(Request?.Headers);
            var response = new ContractResponseBase<KvsSet_ResponseModel>();
            var param = new KvsSet_ParamModel()
            {
                ConfigKey = Request?.Headers?[ConfigConst.ConfigKey],
                VersionDisplay = Request?.Headers?[ConfigConst.Version]
                    ?? ConfigConst.LatestVersion,
                RawData = Request?.GetRawBody(),
                NeedEncrypt = (Request?.Headers?[ConfigConst.NeedEncrypt]).ToBool()
            };

            try
            {
                var result = await m_Service.Execute(param);
                if (result.Any())
                {
                    return response.Success().Content(result);
                }

                return response.Success(StatusCodeEnum.EmptyData);
            }
            catch (Exception ex)
            {
                return response.Error(StatusCodeEnum.Exception, Utility.ErrMsgDependOnEnv(ex));
            }
        }

        protected readonly IKvsSetDirectly_DomainService m_Service;
    }
}
