using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Logging
{
    public class Serilog_Test : TestBase
    {
        public Serilog_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config service")]
        public void AllLevel_Test()
        {
            m_Logger.Information($"this is current time info: {DateTime.UtcNow}");
            m_Logger.Warning($"中文测试");
            m_Logger.Error($"test error. {new Exception("this is exception.")}");
        }

        public override async Task<bool> IsReady()
        {
            var env = AdminApiKeyList.First();
            var configserverUrl = ConfigServiceUrlMap[env.Key];
            Assert.NotNull(configserverUrl);

            var configService = DefaultConfigServer;
            var host = await configService.GetLatest(SysConfigKey.Default_ElasticSearch_HostUrl_ConfigKey);
            Assert.NotNull(host);
            //host = "https://api-dev.kevinw.net/es";
            var option = new ElasticsearchSinkOptions(new Uri(host.Data))
            {
                //ModifyConnectionSettings = conn =>
                //{
                //    var httpConnection = new AwsHttpConnection(awsSettings.Region
                //        , new StaticCredentialsProvider(new AwsCredentials
                //        {
                //            AccessKey = awsSettings.AccessKey,
                //            SecretKey = awsSettings.SecretKey,
                //        }));
                //    var pool = new SingleNodeConnectionPool(new Uri(awsSettings.ElasticSearchUrl));
                //    var conf = new ConnectionConfiguration(pool, httpConnection);
                //    return conf;
                //},
                ConnectionTimeout = new TimeSpan(0, 0, 10),
                IndexFormat = string.Concat(ServiceContext.ApiName, "-{0:yyyy.MM}"),
                FailureCallback = (e, ex) => Console.WriteLine("Unable to submit event " + e.MessageTemplate, ex.ToString()),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                    EmitEventFailureHandling.WriteToFailureSink |
                    EmitEventFailureHandling.RaiseCallback,
                FailureSink = new LoggerConfiguration().WriteTo
                    .RollingFile(new JsonFormatter(), "local-{Date}.txt").CreateLogger()
            };

            m_Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(option)
                .CreateLogger();

            return true;
        }

        protected Serilog.ILogger m_Logger;
    }
}
