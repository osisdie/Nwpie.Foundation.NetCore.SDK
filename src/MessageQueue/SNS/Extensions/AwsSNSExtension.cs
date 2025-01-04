using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.MessageQueue.SNS.Extensions
{
    public static class AwsSNSExtension
    {
        public static IServiceCollection AddSNSService<TService>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IAwsNotificationClient
        {
            // TODO: AwsSNS_Option -> TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new AwsNotificationClient(
                   new ConfigOptions<AwsSNS_Option>(
                        configKey.ConfigServerValue<AwsSNS_Option>()
                   ), p.GetService<ISerializer>()
                ), lifetime));

            return services;
        }
    }
}
