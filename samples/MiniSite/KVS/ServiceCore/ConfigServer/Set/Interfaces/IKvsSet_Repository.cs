using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces
{
    public interface IKvsSet_Repository : IDomainRepository
    {
        Task<string> ExecuteQueryVersion(KvsSet_ParamModel param);
    }
}
