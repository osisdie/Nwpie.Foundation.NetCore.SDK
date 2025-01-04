using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Notification.Endpoint.ServiceCore.Notification.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Notification.Endpoint.ServiceCore.Notification.Interfaces
{
    public interface INtfySend_DomainService : IDomainService
    {
        Task<ResultDtoBase> Execute(NtfySend_ParamModel param);
    }
}
