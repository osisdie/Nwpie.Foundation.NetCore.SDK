using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Measurement
{
    public class Measurement_CloudWatch_Test : TestBase
    {
        public Measurement_CloudWatch_Test(ITestOutputHelper output) : base(output) { }

#if WINDOWS
        [Fact(Skip = "Won't test metrics service")]
        public async Task AmazonCloudWatchSample_Test()
        {
            var percentPageFile = new PerformanceCounter("Paging File", "% Usage", "_Total");
            var peakPageFile = new PerformanceCounter("Paging File", "% Usage Peak", "_Total");

            // Once a minute, send paging file usage statistics to CloudWatch
            for (var i = 0; i < 100; i++)
            {
                var data = new List<MetricDatum>
                {
                    new MetricDatum()
                    {
                        MetricName = "PagingFilePctUsage",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.Percent,
                        Value = percentPageFile.NextValue(),
                        Dimensions = new List<Dimension>()
                        {
                            new Dimension()
                            {
                                Name = CommonConst.ApiName,
                                Value = ServiceContext.ApiName
                            }
                        }
                    },

                    new MetricDatum()
                    {
                        MetricName = "PagingFilePctUsagePeak",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.Percent,
                        Value = peakPageFile.NextValue(),
                        Dimensions = new List<Dimension>()
                        {
                            new Dimension(){Name = CommonConst.ApiName, Value = ServiceContext.ApiName}
                        }
                    }
                };

                await m_LogClient.PutMetricDataAsync(new PutMetricDataRequest()
                {
                    MetricData = data,
                    Namespace = CommonConst.SdkPrefix
                });

                Thread.Sleep(1000 * 10);
            }
        }
#endif

#if WINDOWS
        [Fact]
        //[Fact(Skip = "Won't test metrics service")]
        public void PerformanceCounter_Test()
        {
            var percentPageFile = new PerformanceCounter("Paging File", "% Usage", "_Total");
            var peakPageFile = new PerformanceCounter("Paging File", "% Usage Peak", "_Total");

            // Once a minute, send paging file usage statistics to CloudWatch
            var data = new List<MetricDatum>
            {
                new MetricDatum()
                {
                    MetricName = "PagingFilePctUsage",
                    TimestampUtc = DateTime.UtcNow,
                    Unit = StandardUnit.Percent,
                    Value = percentPageFile.NextValue(),
                    Dimensions = new List<Dimension>()
                    {
                        new Dimension()
                        {
                            Name = CommonConst.ApiName,
                            Value = ServiceContext.ApiName
                        }
                    }
                },

                new MetricDatum()
                {
                    MetricName = "PagingFilePctUsagePeak",
                    TimestampUtc = DateTime.UtcNow,
                    Unit = StandardUnit.Percent,
                    Value = peakPageFile.NextValue(),
                    Dimensions = new List<Dimension>()
                    {
                        new Dimension(){Name = CommonConst.ApiName, Value = ServiceContext.ApiName}
                    }
                }
            };

            Assert.True(data.First().Value > 0);
            Assert.True(data.Last().Value > 0);
        }
#endif

        public override Task<bool> IsReady()
        {
            m_LogClient = new AmazonCloudWatchClient(RegionEndpoint.USWest2);
            Assert.NotNull(m_LogClient);

            return base.IsReady();
        }

        protected IAmazonCloudWatch m_LogClient;
    }
}
