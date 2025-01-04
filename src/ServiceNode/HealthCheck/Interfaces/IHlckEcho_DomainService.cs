using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces
{
    public interface IHlckEcho_DomainService : IDomainService
    {
        Task<HlckEcho_ResponseModel> Execute(HlckEcho_ParamModel param);
    }
}
