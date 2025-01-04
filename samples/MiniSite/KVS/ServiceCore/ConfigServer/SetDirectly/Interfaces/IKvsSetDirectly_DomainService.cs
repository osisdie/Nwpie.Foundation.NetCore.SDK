using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Interfaces
{
    public interface IKvsSetDirectly_DomainService : IDomainService
    {
        Task<KvsSet_ResponseModel> Execute(KvsSet_ParamModel param);
    }
}
