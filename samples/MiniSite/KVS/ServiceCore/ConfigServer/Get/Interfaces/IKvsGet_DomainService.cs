using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces
{
    public interface IKvsGet_DomainService : IDomainService
    {
        Task<KvsGet_ResponseModel> Execute(KvsGet_ParamModel param);
    }
}
