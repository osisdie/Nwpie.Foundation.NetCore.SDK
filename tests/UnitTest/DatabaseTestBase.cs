using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit
{
    public abstract class DatabaseTestBase : TestBase
    {
        public DatabaseTestBase(ITestOutputHelper output) : base(output) { }

        public override async Task<bool> IsReady()
        {
            var connStr_recommend = (SysConfigKey
                .PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                .ConfigServerRawValue();
            Assert.NotNull(connStr_recommend);

            var connStr2 = ServiceContext.Configuration.GetConnectionString(
                SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"
            );

            var connStr3_local = System.Configuration.ConfigurationManager
                .ConnectionStrings[SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"]
                ?.ConnectionString;

            await Task.CompletedTask;
            return true;
        }
    }
}
