using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Location.SDK;

namespace Nwpie.Foundation.Notification.Endpoint.Handlers
{
    public class LocationEventHandler : LocationEventMgr
    {
        public override void OnConsumed(string topic, ICommandModel message)
        {
            base.OnConsumed(topic, message);
        }

        public override void OnConfigRefresh(string configKey)
        {
            if (null == configKey || null == ServiceContext.Configuration[configKey])
            {
                return;
            }
            // TODO: update your config here
        }

        public override void OnLocationRefresh(string serviceName)
        {
            // TODO: update your interested service location
        }
    }
}
