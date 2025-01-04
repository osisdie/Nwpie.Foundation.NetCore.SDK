using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Config.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Notification.SDK.Extensions
{
    public static class NotificationExtension
    {
        public static IServiceCollection AddNotifyHttpClient<TService>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : INotificationHttpClient
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new NotificationHttpClient(
                    configKey.ConfigServerRawValue(),
                    p.GetService<ISerializer>()
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddNotifySQSClient<TService, TOption>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : INotificationSQSClient
            where TOption : AwsSQS_Option
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new NotificationSQSClient(
                    new ConfigOptions<AwsSQS_Option>(
                        configKey.ConfigServerValue<AwsSQS_Option>()
                    ), p.GetService<ISerializer>()
                ), lifetime));

            return services;
        }
    }
}
