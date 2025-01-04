using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.GetDirectly.Interfaces;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.GetDirectly.Services
{
    public class KvsGetDirectly_DomainService :
        DomainService,
        IKvsGetDirectly_DomainService
    {
        public Task<KvsGet_ResponseModel> Execute(KvsGet_ParamModel param)
        {
            Validate(param);

            var service = GetDomainService<IKvsGet_DomainService>();
            service.Headers.CopyFrom(Headers);

            return service.Execute(param);
        }

        public bool Validate(KvsGet_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
