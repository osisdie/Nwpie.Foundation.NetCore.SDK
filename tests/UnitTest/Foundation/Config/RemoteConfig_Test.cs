using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.xUnit.Models;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Config
{
    public class RemoteConfig_Test : TestBase
    {
        public RemoteConfig_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config service")]
        public void Bulk_Test()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .FromConfigServer<string>("foundation.sdk.auth.default")
                .FromConfigServer<string>("foundation.sdk.auth.host_url.default")
                .FromConfigServer<string>("foundation.aws.redis.connectionstring.db0")
                .FromConfigServer<string>("foundation.aws.sqs.urls.notification.default")
                .FromConfigServer<string>("foundation.aws.sqs.urls.measurement.default")
                .FromConfigServer<string>("foundation.aws.sns.urls.location.default")
                .FromConfigServer<string>("foundation.notification.line.default")
                .FromConfigServer<string>("foundation.notification.smtp.default")
                .FromConfigServer<string>("foundation.notification.slack.default")
                .FromConfigServer<string>("foundation.service.health.get_url.all")
                .FromConfigServer<string>("foundation.service.elasticsearch.host_url.default")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.sys_db")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.fruit_db")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.assets_db")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.todo_db")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.auth_db")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.svc_ds1")
                .FromConfigServer<string>("foundation.aws.mysql.connectionstring.svc_crawler")
               ;

            var sw = new Stopwatch();
            sw.Start();
            IConfiguration configuration = builder.Build();
            Assert.True(sw.Elapsed.TotalSeconds < ConfigConst.DefaultHttpTimeout);

            Assert.NotNull("foundation.sdk.auth.default".ConfigServerRawValue());
            Assert.NotNull("foundation.sdk.auth.host_url.default".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.redis.connectionstring.db0".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.sqs.urls.notification.default".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.sqs.urls.measurement.default".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.sns.urls.location.default".ConfigServerRawValue());
            Assert.NotNull("foundation.notification.line.default".ConfigServerRawValue());
            Assert.NotNull("foundation.notification.smtp.default".ConfigServerRawValue());
            Assert.NotNull("foundation.notification.slack.default".ConfigServerRawValue());
            Assert.NotNull("foundation.service.health.get_url.all".ConfigServerRawValue());
            Assert.NotNull("foundation.service.elasticsearch.host_url.default".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.sys_db".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.fruit_db".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.assets_db".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.todo_db".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.auth_db".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.svc_ds1".ConfigServerRawValue());
            Assert.NotNull("foundation.aws.mysql.connectionstring.svc_crawler".ConfigServerRawValue());
        }

        [Fact]
        public void Overwrite_IConfiguration_Test()
        {
            var setRuntimeConfigValue = IdentifierUtils.RandomNumber(10);
            ServiceContext.Configuration["Runtime"] = setRuntimeConfigValue;
            Assert.Equal(setRuntimeConfigValue, ServiceContext.Configuration["Runtime"]);

            var hostUrlOld = SysConfigKey
                .Default_Auth_HostUrl_ConfigKey
                .ConfigServerRawValue();
            Assert.NotNull(hostUrlOld);

            var newVal = "about:blank";
            ServiceContext.Configuration[SysConfigKey.Default_Auth_HostUrl_ConfigKey] = newVal;
            var hostUrlNew = SysConfigKey
                .Default_Auth_HostUrl_ConfigKey
                .ConfigServerRawValue();
            Assert.Equal(newVal, hostUrlNew);

            ServiceContext.Configuration[SysConfigKey.Default_Auth_HostUrl_ConfigKey] = hostUrlOld;
        }

        [Fact(Skip = "Won't test remote config service")]
        public async Task GetConfig_ViaCustomClient_Test()
        {
            var configService = DefaultConfigServer;

            configService.GetUrl = "https://api.kevinw.net/foundation".ResolveGetActionUrl("configserver");
            configService.SetUrl = "https://api.kevinw.net/foundation".ResolveSetActionUrl("configserver");

            var response = await configService.GetLatest<SvcDS1_AwsCfg>("ds1.prod");
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
        }

        [Fact(Skip = "Won't test remote config service")]
        public async Task GetConfig_ViaOriginalClient_Test()
        {
            var configService = DefaultConfigServer;

            try
            {
                var response = await configService.GetLatest<SvcDS1_AwsCfg>("ds1.prod");
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.NotNull(response.Data);
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }
    }
}
