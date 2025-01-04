using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Measurement.SDK.Extensions
{
    public static class MeasurementExtension
    {
        public static IServiceCollection AddMetricService<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IMeasurement
        {
            services.Add(new ServiceDescriptor(typeof(TService),
                typeof(MetricClient), lifetime));
            return services;
        }
    }
}
