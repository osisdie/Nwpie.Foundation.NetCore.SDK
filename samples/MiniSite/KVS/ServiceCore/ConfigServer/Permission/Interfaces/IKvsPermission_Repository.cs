using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces
{
    public interface IKvsPermission_Repository : IDomainRepository
    {
        Task<string> ExecuteQueryVersion(KvsPermission_ParamModel param);
    }
}
