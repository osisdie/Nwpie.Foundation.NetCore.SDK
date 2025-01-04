using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces
{
    public interface IHlckEcho_Repository : IDomainRepository<HlckEcho_Entity>
    {
        Task<string> ExecuteQueryVersion(HlckEcho_ParamModel param);
        Task<HlckEcho_Entity> ExecuteQuery(HlckEcho_ParamModel param);
    }
}
