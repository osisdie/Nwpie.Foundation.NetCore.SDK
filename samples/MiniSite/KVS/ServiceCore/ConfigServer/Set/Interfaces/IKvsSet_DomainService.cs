using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces
{
    public interface IKvsSet_DomainService : IDomainService
    {
        Task<KvsSet_ResponseModel> Execute(KvsSet_ParamModel param);
    }
}
