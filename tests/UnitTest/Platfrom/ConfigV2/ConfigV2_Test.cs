using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.ConfigV2
{
    public class ConfigV2_Test : TestBase
    {
        public ConfigV2_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Location_ConfigGet_Test()
        {
            ServiceContext.ConfigServiceUrl = "https://api.kevinw.net/loc/configserver";
            var apiName = m_ApiName;
            var apiKey = m_ApiKey;

            var client = new DefaultRemoteConfigClient(Serializer);
            var result = await client.GetLatest(
                m_CurrentProcessConfigKeys.Select(o => new ConfigItem { ConfigKey = o }).ToList(),
                apiName: apiName,
                apiKey: apiKey
            );

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.First().Value);
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task V1_ConfigGet_Test()
        {
            ServiceContext.ConfigServiceUrl = "https://api.kevinw.net/foundation/configserver";
            var apiName = m_ApiName;
            var apiKey = m_ApiKey;

            var client = new DefaultRemoteConfigClient(Serializer);
            var result = await client.GetLatest(
                m_CurrentProcessConfigKeys.Select(o => new ConfigItem { ConfigKey = o }).ToList(),
                apiName: apiName,
                apiKey: apiKey
            );
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.First().Value);
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task V2_ConfigGet_Test()
        {
            ServiceContext.ConfigServiceUrl = "https://api.kevinw.net/config";
            var apiName = m_ApiName;
            var apiKey = m_ApiKey;

            var client = new DefaultRemoteConfigClient(Serializer);
            var result = await client.GetLatest(
                m_CurrentProcessConfigKeys.Select(o => new ConfigItem { ConfigKey = o }).ToList(),
                apiName: apiName,
                apiKey: apiKey
            );
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.First().Value);
        }

        public override Task<bool> IsReady()
        {
            DefaultRemoteConfigClient.GlobalTimeoutSecs = 60;

            return base.IsReady();
        }

        //protected string m_ConfigServiceUrl = "https://api.kevinw.net/loc/configserver";
        protected string m_ConfigServiceUrl = "https://api.kevinw.net/config";
        protected string m_ApiName = "auth.service.prod";
        protected string m_ApiKey = "**";

        protected List<string> m_CurrentProcessConfigKeys = new()
        {
             "foundation.sdk.auth.default",
             "foundation.sdk.auth.host_url.default",
             "foundation.aws.mysql.connectionstring.sys_db",
             "foundation.aws.redis.connectionstring.db0",
             "foundation.aws.sqs.urls.notification.default",
             "foundation.aws.sqs.urls.measurement.default",
             "foundation.aws.sns.urls.location.default",
             "foundation.service.notification.host_url.default",
        };
    }
}
