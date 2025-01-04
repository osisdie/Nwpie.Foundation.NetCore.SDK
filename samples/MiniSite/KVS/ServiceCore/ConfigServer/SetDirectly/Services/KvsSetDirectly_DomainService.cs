using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;
using Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Interfaces;

namespace Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Services
{
    public class KvsSetDirectly_DomainService :
        DomainService,
        IKvsSetDirectly_DomainService
    {
        public Task<KvsSet_ResponseModel> Execute(KvsSet_ParamModel param)
        {
            Validate(param);

            var service = GetDomainService<IKvsSet_DomainService>();
            service.Headers.CopyFrom(Headers);

            return service.Execute(param);
        }

        public bool Validate(KvsSet_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
