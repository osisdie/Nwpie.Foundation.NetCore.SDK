using System;
using System.Threading.Tasks;
using System.Transactions;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class CommandRollback_Test : DatabaseTestBase
    {
        public CommandRollback_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test dal service")]
        public async Task Update_Complete_Test()
        {
            var now = DateTime.UtcNow;
            var ran = new Random().Next(1, 100);

            var param = new TestTable_Entity()
            {
                ColumnChar = $"{m_Id}-Update_Complete_Test-測試",
                ColumnInt = ran,
                ColumnDecimal = 1.1M,
                ColumnBool = ran % 2 == 1,
                ColumnDate = DateTime.UtcNow,
                ColumnDatetime = now
            };

            {
                var cmd = new CommandExecutor("Unittest:testtable:findColumnChar");
                cmd.SetParameterValue("columnChar", param.ColumnChar);
                var result = await cmd.ExecuteEntityListAsync<TestTable_Entity>();
                Assert.Null(result);
            }

            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var cmd = new CommandExecutor("Unittest:testtable:insert");

                cmd.SetParameterValue("columnChar", param.ColumnChar);
                cmd.SetParameterValue("columnInt", param.ColumnInt);
                //cmd.SetParameterValue("columnInt", param.columnBool);
                cmd.SetParameterValue("columnDecimal", param.ColumnDecimal);
                cmd.SetParameterValue("columnBool", param.ColumnBool);
                cmd.SetParameterValue("columnDate", param.ColumnDatetime);
                cmd.SetParameterValue("columnDatetime", now);

                var result = await cmd.ExecuteEntityAsync<TestTable_Entity>();
                transactionScope.Complete();

                Assert.NotNull(result);
                Assert.Equal(param.ColumnChar, result.ColumnChar);
                Assert.Equal(param.ColumnInt, result.ColumnInt);
                Assert.Equal(param.ColumnDecimal, result.ColumnDecimal);
                Assert.Equal(param.ColumnBool, result.ColumnBool);
                Assert.Equal(param.ColumnDate.Value.Year, result.ColumnDate.Value.Year);
                Assert.Equal(param.ColumnDate.Value.Month, result.ColumnDate.Value.Month);
                Assert.Equal(param.ColumnDate.Value.Day, result.ColumnDate.Value.Day);
            }

            {
                var cmd = new CommandExecutor("Unittest:testtable:findColumnChar");
                cmd.SetParameterValue("columnChar", param.ColumnChar);
                var result = await cmd.ExecuteEntityListAsync<TestTable_Entity>();
                Assert.NotEmpty(result);
            }
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task Update_Rolloback_Test()
        {
            var now = DateTime.UtcNow;
            var ran = new Random().Next(1, 100);

            var param = new TestTable_Entity()
            {
                ColumnChar = $"{m_Id}-Update_Rolloback_Test-測試",
                ColumnInt = ran,
                ColumnDecimal = 1.1M,
                ColumnBool = ran % 2 == 1,
                ColumnDate = DateTime.UtcNow,
                ColumnDatetime = now
            };

            {
                var cmd = new CommandExecutor("Unittest:testtable:findColumnChar");
                cmd.SetParameterValue("columnChar", param.ColumnChar);
                var result = await cmd.ExecuteEntityListAsync<TestTable_Entity>();
                Assert.Null(result);
            }

            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var cmd = new CommandExecutor("Unittest:testtable:insert");

                cmd.SetParameterValue("columnChar", param.ColumnChar);
                cmd.SetParameterValue("columnInt", param.ColumnInt);
                //cmd.SetParameterValue("columnInt", param.columnBool);
                cmd.SetParameterValue("columnDecimal", param.ColumnDecimal);
                cmd.SetParameterValue("columnBool", param.ColumnBool);
                cmd.SetParameterValue("columnDate", param.ColumnDatetime);
                cmd.SetParameterValue("columnDatetime", now);

                var result = await cmd.ExecuteEntityAsync<TestTable_Entity>();
                transactionScope.Dispose();
            }

            // Still Not Found
            {
                var cmd = new CommandExecutor("Unittest:testtable:findColumnChar");
                cmd.SetParameterValue("columnChar", param.ColumnChar);
                var result = await cmd.ExecuteEntityListAsync<TestTable_Entity>();
                Assert.Null(result);
            }
        }

        protected readonly string m_Id = IdentifierUtils.NewId();
    }
}
