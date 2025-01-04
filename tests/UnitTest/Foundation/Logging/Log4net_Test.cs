using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Logging
{
    public class Log4net_Test : TestBase
    {
        public Log4net_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void AllLevel_Test()
        {
            Logger.LogInformation($"this is current time info: {DateTime.UtcNow}");
            Logger.LogWarning($"中文测试");
            Logger.LogError($"test error. {new Exception("this is exception.")}");
        }
    }
}
