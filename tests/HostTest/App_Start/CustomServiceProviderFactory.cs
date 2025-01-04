using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.HostTest.App_Start
{
    public class CustomServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly ServiceProviderOptions _options = new ServiceProviderOptions
        {
            ValidateOnBuild = false, // default
            ValidateScopes = false // default
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
        /// with default options.
        /// </summary>
        /// <seealso cref="ServiceProviderOptions.Default"/>
        //public CustomServiceProviderFactory()
        //{

        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
        /// with the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options to use for this instance.</param>
        public CustomServiceProviderFactory(ServiceProviderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
        }

        /// <inheritdoc />
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvider"/> class
        /// with the specified <paramref name="containerBuilder"/>.
        /// </summary>
        /// <param name="containerBuilder">The IServiceCollection to use for this instance.</param>
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider(_options);

            // TODO: here you go
            //var hostedService = serviceProvider.GetService<IHostedService>();

            return serviceProvider;
        }
    }
}
