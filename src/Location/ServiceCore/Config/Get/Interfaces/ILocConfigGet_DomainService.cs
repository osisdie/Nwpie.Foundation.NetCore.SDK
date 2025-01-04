using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Config.Get;
using Nwpie.Foundation.Location.ServiceCore.Config.Get.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Get.Interfaces
{
    public interface ILocConfigGet_DomainService : IDomainService
    {
        Task<LocConfigGet_ResponseModel> Execute(LocConfigGet_ParamModel param);
    }
}
