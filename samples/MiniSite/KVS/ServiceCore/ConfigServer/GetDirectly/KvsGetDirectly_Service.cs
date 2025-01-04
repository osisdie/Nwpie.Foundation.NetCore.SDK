using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.Common.Attributes;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.Contract.Configserver.GetDirectly;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.GetDirectly.Interfaces;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.GetDirectly
{
    [CustomApiKeyFilter]
    public class KvsGetDirectly_Service : HttpRequestServiceBase
    {
        public KvsGetDirectly_Service(IKvsGetDirectly_DomainService service)
        {
            m_Service = service ?? throw new ArgumentNullException(nameof(IKvsGetDirectly_DomainService));
        }

        public async Task<object> Any(KvsGetDirectly_Request request)
        {
            m_Service.Headers.CopyFrom(Request?.Headers);

            var param = new KvsGet_ParamModel
            {
                ConfigKeys = new List<KvsGet_RequestModelItem>()
                {
                    new KvsGet_RequestModelItem
                    {
                        ConfigKey = Request?.Headers?[ConfigConst.ConfigKey],
                        Version = Request?.Headers?[ConfigConst.Version]
                            ?? ConfigConst.LatestVersion
                    }
                }
            };

            try
            {
                var result = await m_Service.Execute(param);
                if (result.Any())
                {
                    return result.RawData.First().Value;
                }

                throw new Exception($"Config (key={Request?.Headers?[ConfigConst.ConfigKey]}) not exists. ");
            }
            catch (Exception ex)
            {
                base.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return new ServiceResponse().Error(StatusCodeEnum.Exception, ex);
            }
        }

        protected readonly IKvsGetDirectly_DomainService m_Service;
    }
}
