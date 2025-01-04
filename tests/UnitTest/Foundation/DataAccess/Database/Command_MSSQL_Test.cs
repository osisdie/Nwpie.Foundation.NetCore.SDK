using System;
using System.Threading.Tasks;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.DataAccess.Database.Measurement;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class Command_MSSQL_Test : DatabaseTestBase
    {
        public Command_MSSQL_Test(ITestOutputHelper output) : base(output)
        {
            CommandExecutorExtension.RemoteLoggingEnabled = false;
            CommandExecutorExtension.MeasurementEnabled = false;
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ShowVersion_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:show:version:SkyDb");
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
                var cmd = new CommandExecutor("Unittest:show:databases:SkyDb");
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
                var cmd = new CommandExecutor("Unittest:show:tables:SkyDb");
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
                var cmd = new CommandExecutor("Unittest:show:schema:by:table:SkyDb");
                cmd.SetParameterValue("TableName", "COMP_Bank");
                var result = await cmd.ExecuteEntityListAsync<DbInformationSchema>();
                Assert.NotEmpty(result);
                Assert.Equal(6, result.Count);
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecSP_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:exec:SP:SkyDb");

                cmd.ToggleDynamicSection("D_say", true);
                cmd.SetParameterValue("_Say", "unittest");
                var result = await cmd.ExecuteEntityAsync<DbExecSPResult>();
                Assert.NotNull(result);
                Assert.Equal("unittest", result.Say);
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

        public class DbExecSPResult
        {
            public string Say { get; set; }
            public string Version { get; set; }
        }

    }
}
