using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Logging
{
    public class LoggerExtension_Test : TestBase
    {
        public LoggerExtension_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void AddTraceData_Test()
        {
            // <string, string>
            {
                var collection = LoggerExtension.BeginCollection<string>()
                    .TryAdd("key1", "value1")
                    .AddTraceData(DateTime.UtcNow.AddSeconds(-1));

                Assert.True(collection.Count() > 1);
                Assert.True(collection.ContainsKey(SysLoggerKey.ElapsedSeconds));
                Assert.True(collection.ContainsKey(SysLoggerKey.ApiKey));
                Assert.True(collection.ContainsKey(SysLoggerKey.ApiName));
                Assert.True(collection.ContainsKey(SysLoggerKey.Service));
                Assert.True(collection.ContainsKey(SysLoggerKey.Version));
                Assert.True(collection.ContainsKey(SysLoggerKey.Environment));
                Assert.True(collection.ContainsKey(SysLoggerKey.IP));
                Assert.True(collection.ContainsKey(SysLoggerKey.HostName));
                Assert.True(collection.ContainsKey(SysLoggerKey.Platform));
                Assert.True(collection.ContainsKey(SysLoggerKey.TimeStamp));
                Assert.True(collection.ContainsKey(SysLoggerKey.UpTime));
            }

            // <string, object>
            {
                var collection = LoggerExtension.BeginCollection<object>()
                    .TryAdd("key1", "value1")
                    .AddTraceData(DateTime.UtcNow.AddSeconds(-1));

                Assert.True(collection.Count() > 1);
                Assert.True(collection.ContainsKey(SysLoggerKey.ElapsedSeconds));
                Assert.True(collection.ContainsKey(SysLoggerKey.ApiKey));
                Assert.True(collection.ContainsKey(SysLoggerKey.ApiName));
                Assert.True(collection.ContainsKey(SysLoggerKey.Service));
                Assert.True(collection.ContainsKey(SysLoggerKey.Version));
                Assert.True(collection.ContainsKey(SysLoggerKey.Environment));
                Assert.True(collection.ContainsKey(SysLoggerKey.IP));
                Assert.True(collection.ContainsKey(SysLoggerKey.HostName));
                Assert.True(collection.ContainsKey(SysLoggerKey.Platform));
                Assert.True(collection.ContainsKey(SysLoggerKey.TimeStamp));
                Assert.True(collection.ContainsKey(SysLoggerKey.UpTime));
            }
        }
    }
}
