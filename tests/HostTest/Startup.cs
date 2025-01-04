using Nwpie.HostTest.Extensions;
using Nwpie.HostTest.Implements;
using Nwpie.HostTest.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nwpie.HostTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();
            //services.AddHostedService(p =>
            //{
            //    return new ApplicationStart();
            //});
            //services.AddSingleton<IHostedService, ApplicationStart>();

            services.AddScoped<IRepository, ScalarStringRepository>();
            services.AddScoped<IRepository, UserProfileRepository>();
            services.AddDerivedRepository(ServiceLifetime.Scoped);

            // MVC
            services.AddHealthChecks();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
