using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Contracts;

namespace Nwpie.Foundation.Abstractions.Notification.Interfaces
{
    public interface INotificationHttpClient : IClient
    {
        Task<bool> IsReady();
        Task<NotifySend_Response> SendAsync(NotifySend_Request request);
        string NotificationHostUrl { get; set; }
        int DefaultTimeoutSecs { get; set; }
        int DefaultRetries { get; set; }
        int DefaultDelayRetrySecs { get; set; }
    }
}
