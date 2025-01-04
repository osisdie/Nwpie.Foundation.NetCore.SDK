using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Contracts;

namespace Nwpie.Foundation.Abstractions.Notification.Interfaces
{
    public interface INotificationSQSClient : IClient
    {
        Task<bool> IsReady();
        Task<IServiceResponse> SendAsync(NotifySend_RequestModel request);
    }
}
