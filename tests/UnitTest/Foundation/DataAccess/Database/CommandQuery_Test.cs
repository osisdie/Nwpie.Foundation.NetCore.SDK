using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess.Entities;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.DataAccess.Database.Measurement;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class CommandQuery_Test : DatabaseTestBase
    {
        public CommandQuery_Test(ITestOutputHelper output) : base(output)
        {
            CommandExecutorExtension.RemoteLoggingEnabled = false;
            CommandExecutorExtension.MeasurementEnabled = true;
        }

        [Fact(Skip = "Won't test dal service")]
        public void ConnectionString_Test()
        {
            var connStr_recommend = (
                SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"
            ).ConfigServerRawValue();
            Assert.NotNull(connStr_recommend);

            var connStr2 = ServiceContext.Configuration.GetConnectionString(
                SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"
            );
            var connStr3_local = System.Configuration.ConfigurationManager
                .ConnectionStrings[SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db"]
                ?.ConnectionString;
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteTimeout_GetDBVersion_Test()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:echo:timeout");
                var result = await cmd.ExecuteScalarAsync<string>();
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteNoQuery_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteNonQueryAsync();
            Assert.Equal(ConfigConst.DefaultDBExecutionResult, result);
            Assert.NotNull(cmd.CurrentCommand.CommandText);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteScalar_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteScalarAsync<string>();
            Assert.Matches(m_ExpectMySQLVersion, result);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteDynamic_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteDynamicAsync();
            Assert.NotNull(result);
            Assert.Matches(m_ExpectMySQLVersion, result.version);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteDynamicList_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteDynamicListAsync();
            Assert.Single(result);
            Assert.Matches(m_ExpectMySQLVersion, result.First().version);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteEntity_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteEntityAsync<DBVersion_Entity>();
            Assert.NotNull(result);
            Assert.Matches(m_ExpectMySQLVersion, result.version);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteEntity_GetNull_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:null");
            var result = await cmd.ExecuteEntityAsync<DBVersion_Entity>();
            Assert.Null(result);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteList_MySQL_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteListAsync<string>();
            Assert.Single(result);
            Assert.Matches(m_ExpectMySQLVersion, result.First());
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteEntityList_MySQL_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            var result = await cmd.ExecuteEntityListAsync<DBVersion_Entity>();
            Assert.Single(result);
            Assert.Matches(m_ExpectMySQLVersion, result.First().version);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteEntityList_GetNull_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:null");
            var result = await cmd.ExecuteEntityListAsync<DBVersion_Entity>();
            Assert.Empty(result);
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task ExecuteReader_MySQL_GetDBVersion_Test()
        {
            var cmd = new CommandExecutor("Unittest:echo:version");
            await cmd.ExecuteReaderAsync((r) =>
            {
                Assert.False(r.IsClosed);
                while (r.Read())
                {
                    Assert.NotNull(r.GetValue(0));
                    Assert.Matches(m_ExpectMySQLVersion, r.GetValue(0).ToString());
                }
            });
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task Connection_MySQL_GUID_Test()
        {
            {
                var cmd = new CommandExecutor("Unittest:echo:version");
                var result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid1 = cmd.ConnectionGuid;
                Assert.NotNull(connGuid1);

                result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid2 = cmd.ConnectionGuid;
                Assert.NotEqual(connGuid1, connGuid2);
            }

            // reuse command and its connection
            {
                var cmd = new CommandExecutor("Unittest:echo:version")
                {
                    IsDisposePerExecution = false
                };
                var result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid1 = cmd.ConnectionGuid;
                Assert.NotNull(connGuid1);

                result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid2 = cmd.ConnectionGuid;
                Assert.Equal(connGuid1, connGuid2);
            }
        }

        [Fact(Skip = "Skip performance test(Slowest)")]
        public async Task CommandLoop_MySQL_Performance_Test()
        {
            var count = LOOP_COUNT; // 100: 1 min
            var tick = DateTime.UtcNow;
            Guid? guid = null;
            for (var i = 0; i < count; i++)
            {
                var cmd = new CommandExecutor("Unittest:echo:version");
                var result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid = cmd.ConnectionGuid;
                Assert.NotNull(connGuid);

                if (null == guid)
                {
                    guid = connGuid;
                    continue;
                }
                Assert.NotEqual(guid, connGuid);
            }

            Console.WriteLine($"ellapsedSeconds: {(DateTime.UtcNow - tick).TotalSeconds}");
        }

        [Fact(Skip = "Skip performance test (Fatest)")]
        public async Task CommandLoop_MySQL_ReuseConnection_Performance_Test()
        {
            var count = LOOP_COUNT; // 100: 22 sec
            var tick = DateTime.UtcNow;
            var cmd = new CommandExecutor("Unittest:echo:version")
            {
                IsDisposePerExecution = false // reuse command and its connection
            };

            Guid? guid = null;
            for (var i = 0; i < count; i++)
            {
                var result = await cmd.ExecuteScalarAsync<string>();
                Assert.Matches(m_ExpectMySQLVersion, result);
                var connGuid = cmd.ConnectionGuid;
                Assert.NotNull(connGuid);

                if (null == guid)
                {
                    guid = connGuid;
                }
                Assert.Equal(guid, connGuid);
            }

            Console.WriteLine($"ellapsedSeconds: {(DateTime.UtcNow - tick).TotalSeconds}");
        }

        [Fact(Skip = "Skip performance test (Fatest)")]
        public void Client_MySQL_ReuseConnection_Performance_Test()
        {
            var count = LOOP_COUNT; // 100: 23 sec

            var commandText = @"SELECT VERSION() as `version`;";
            DbConnection cnn = new MySqlConnection(
                (SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                    .ConfigServerRawValue()
            );

            using (cnn)
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = commandText;
                for (var i = 0; i < count; i++)
                {
                    var result = cmd.ExecuteScalar();
                    Assert.NotNull(result);
                    Assert.Matches(m_ExpectMySQLVersion, result.ToString());
                }
            }
        }

        [Fact(Skip = "Skip performance test (Fatest)")]
        public void Client_MsSQL_ReuseConnection_Performance_Test()
        {
            var count = LOOP_COUNT; // 100: 23 sec

            var commandText = @"SELECT @@VERSION as 'version';";
            DbConnection cnn = new SqlConnection(
                (SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                    .ConfigServerRawValue()
            );

            using (cnn)
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = commandText;
                for (var i = 0; i < count; i++)
                {
                    var result = cmd.ExecuteScalar();
                    Assert.NotNull(result);
                    Assert.Matches(m_ExpectMsSQLVersion, result.ToString());
                }
            }
        }

        private const int LOOP_COUNT = 100;

        protected readonly Regex m_ExpectMySQLVersion = new Regex("^(8.0).[0-9]+"); // RDS or MySQL
        // Microsoft SQL Server 2019 (RTM-CU12) (KB5004524) - 15.0.4153.1 (X64)   Jul 19 2021 15:37:34   Copyright (C) 2019 Microsoft Corporation  Express Edition (64-bit) on Windows 10 Pro 10.0 <X64> (Build 22631: ) (Hypervisor)
        protected readonly Regex m_ExpectMsSQLVersion = new Regex("^(15.0).[0-9]+");
    }
}
