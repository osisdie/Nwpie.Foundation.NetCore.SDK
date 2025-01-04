using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Config.Refresh;
using Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Interfaces
{
    public interface ILocConfigRefresh_DomainService : IDomainService
    {
        Task<LocConfigRefresh_ResponseModel> Execute(LocConfigRefresh_ParamModel param);
    }
}
