using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Config
{
    public class AppSettings_Test : TestBase
    {
        public AppSettings_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ConnectionString_Test()
        {
            {
                var conn = ServiceContext.Configuration
                    .GetConnectionString("I'm not exists");
                Assert.Null(conn);
            }

            {
                var conn = ServiceContext.Configuration
                    .GetConnectionString(
                        SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"
                    );
                Assert.NotNull(conn);
            }

            {
                var conns = ServiceContext.Configuration
                    .GetSection("ConnectionStrings").GetChildren();
                Assert.NotEmpty(conns);
                foreach (var item in conns)
                {
                    Assert.NotNull(item.Key);
                    Assert.NotNull(item.Value);
                }
            }
        }
    }
}
