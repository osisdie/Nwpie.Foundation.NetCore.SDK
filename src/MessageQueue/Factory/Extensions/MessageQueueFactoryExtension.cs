using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.MessageQueue.Factory.Extensions
{
    public static class MessageQueueFactoryExtension
    {
        public static IServiceCollection AddMessageQueueFactory<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IMessageQueueFactory
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new MessageQueueFactory(
                    p.GetService<ISerializer>()
                ), lifetime));

            return services;
        }
    }
}
