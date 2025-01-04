using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.Contract.Configserver.Permission;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces
{
    public interface IKvsPermission_DomainService : IDomainService
    {
        Task<KvsPermission_ResponseModel> Execute(KvsPermission_ParamModel param);
    }
}
