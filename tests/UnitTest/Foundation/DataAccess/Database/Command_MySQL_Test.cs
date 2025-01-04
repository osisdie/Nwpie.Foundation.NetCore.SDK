using System;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.DataAccess.Database.Measurement;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class Command_MySQL_Test : DatabaseTestBase
    {
        public Command_MySQL_Test(ITestOutputHelper output) : base(output)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            CommandExecutorExtension.RemoteLoggingEnabled = false;
            CommandExecutorExtension.MeasurementEnabled = false;
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ShowVersion_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:show:version:sys_db");
                var result = await cmd.ExecuteScalarAsync<string>();
                Assert.NotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ShowDatabases_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:show:databases:sys_db");
                var result = await cmd.ExecuteListAsync<string>();
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ShowTables_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:show:tables:sys_db");
                var result = await cmd.ExecuteListAsync<string>();
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ShowSchemaByTable_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:show:schema:by:table:sys_db");
                cmd.SetParameterValue("table_name", "test_table");
                var result = await cmd.ExecuteEntityListAsync<DbInformationSchema>();
                Assert.NotEmpty(result);
                Assert.Equal(12, result.Count);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task Parameter_Bool_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:query:todo_db:TestTable");
                cmd.SetParameterValue("offset", 0);
                cmd.SetParameterValue("limit", 1);
                if (true)
                {
                    cmd.ToggleDynamicSection("D_columnBool", true);
                    cmd.SetParameterValue("columnBool", true);
                }
                var result = await cmd.ExecuteEntityListAsync<TestTable_Entity>();
                Assert.Single(result);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        // DESCRIBE `ACCOUNT`;
        public class DbScribeSchema
        {
            public string Field { get; set; }
            public string Type { get; set; }
            public string Null { get; set; }
            public string Key { get; set; }
            public string Default { get; set; }
            public string Extra { get; set; }
        }

        public class DbInformationSchema
        {
            public string COLUMN_NAME { get; set; }
            public string COLUMN_DEFAULT { get; set; }
            public string IS_NULLABLE { get; set; }
            public string DATA_TYPE { get; set; }
            public string COLUMN_TYPE { get; set; }
            public string COLUMN_KEY { get; set; }
            public string EXTRA { get; set; }
        }
    }
}
