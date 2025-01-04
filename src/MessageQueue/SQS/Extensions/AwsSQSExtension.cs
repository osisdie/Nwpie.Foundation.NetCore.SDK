using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.MessageQueue.SQS.Extensions
{
    public static class AwsSQSExtension
    {
        public static IServiceCollection AddSQSService<TService>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IAwsSQSClient
        {
            // TODO: AwsSQS_Option -> TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new AwsSQSClient(
                    new ConfigOptions<AwsSQS_Option>(
                        configKey.ConfigServerValue<AwsSQS_Option>()
                    ), p.GetService<ISerializer>()
                ), lifetime));

            return services;
        }
    }
}
