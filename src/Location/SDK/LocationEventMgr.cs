using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Models;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Location.SDK
{
    public class LocationEventMgr : SingleCObject<LocationEventMgr>
    {
        protected override void InitialInConstructor()
        {
            Serializer = new DefaultSerializer();
        }

        public virtual void OnConsumed(string topic, ICommandModel message)
        {
            Logger.LogInformation($"Receive from topic(={topic}), msg:{message?.Raw}");

            if (string.IsNullOrWhiteSpace(message?.Raw))
            {
                return;
            }

            var cmdDto = Serializer.Deserialize<CommandModel>(message.Raw, ignoreException: true);
            if (null == cmdDto)
            {
                return;
            }

            if (true == cmdDto.Topic?.Equals(LocationConst.HttpPathToConfigContractRequest_Refresh, System.StringComparison.OrdinalIgnoreCase))
            {
                OnConfigRefresh(cmdDto);
            }

            if (true == cmdDto.Topic?.Equals(LocationConst.HttpPathToLocationContractRequest_Refresh, System.StringComparison.OrdinalIgnoreCase))
            {
                OnLocationRefresh(cmdDto);
            }
        }

        public virtual void OnConfigRefresh(CommandModel message)
        {
            var configs = Serializer.Deserialize<List<string>>(message?.Data);
            if (true == configs?.Count() > 0)
            {
                foreach (var configKey in configs)
                {
                    //if (null == configKey || null == ServiceContext.Configuration[configKey])
                    //    continue;

                    // TODO: update your config here
                    OnConfigRefresh(configKey);
                }
            }
        }

        public virtual void OnConfigRefresh(string configKey) { }
        public virtual void OnLocationRefresh(string serviceName) { }

        public virtual void OnLocationRefresh(CommandModel message)
        {
            var services = Serializer.Deserialize<List<string>>(message?.Data);
            if (true == services?.Count() > 0)
            {
                foreach (var svc in services)
                {
                    // TODO: update your interested service location
                    OnLocationRefresh(svc);
                }
            }
        }

        public override void Dispose()
        {

        }

        protected ISerializer Serializer { get; private set; }
    }
}
