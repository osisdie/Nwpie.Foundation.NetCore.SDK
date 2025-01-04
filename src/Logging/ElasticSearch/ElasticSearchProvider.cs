using System;
using System.IO;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;

namespace Nwpie.Foundation.Logging.ElasticSearch
{
    public class ElasticSearchProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        public ElasticSearchProvider() : this(null) { }
        public ElasticSearchProvider(ElasticsearchSinkOptions options)
        {
            Setup(options);
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) =>
            new ElasticSearchAdapter(m_SinkOption);

        public void Dispose() { }

        // Fill Option from config server
        void Setup(ElasticsearchSinkOptions options = null)
        {
            using (var configService = new DefaultRemoteConfigClient(new DefaultSerializer()))
            {
                var response = configService.GetLatest(
                        SysConfigKey.Default_ElasticSearch_HostUrl_ConfigKey
                    ).ConfigureAwait(false).GetAwaiter().GetResult();

                m_Option = new ElasticSearch_Option()
                {
                    Host = response?.Data, // Exception later if Host isNullOrEmpty
                    ConnectionTimeout = 10,
                    IndexFormat = $"{ServiceContext.ApiName}-{0:yyyy.MM}"
                };
            }

            m_SinkOption = options ?? new ElasticsearchSinkOptions(new Uri(m_Option.Host))
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
                ConnectionTimeout = new TimeSpan(0, 0, m_Option.ConnectionTimeout),
                IndexFormat = string.Concat(ServiceContext.ApiName, "-{0:yyyy.MM}"),
                FailureCallback = (e, ex) => Console.WriteLine("Unable to submit event " + e.MessageTemplate, ex.ToString()),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                    EmitEventFailureHandling.WriteToFailureSink |
                    EmitEventFailureHandling.RaiseCallback,
                FailureSink = new LoggerConfiguration().WriteTo
                    .RollingFile(new JsonFormatter(), "local-{Date}.txt").CreateLogger()
            };
        }

        public FileInfo ProviderFile { get; private set; }

        protected ElasticSearch_Option m_Option;
        protected ElasticsearchSinkOptions m_SinkOption;
    }
}
