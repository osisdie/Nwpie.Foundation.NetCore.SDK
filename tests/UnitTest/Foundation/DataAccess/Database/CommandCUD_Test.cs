using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class CommandCUD_Test : DatabaseTestBase
    {
        public CommandCUD_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test dal service")]
        public async Task InsertTestTable_ReturnEntity_Test()
        {
            var now = DateTime.UtcNow;
            var ran = new Random().Next(1, 100);

            var param = new TestTable_Entity()
            {
                ColumnChar = $"{ran}-這是中文",
                ColumnInt = ran,
                ColumnDecimal = 1.1M,
                ColumnBool = ran % 2 == 1,
                ColumnDate = DateTime.UtcNow,
                ColumnDatetime = now
            };

            var cmd = new CommandExecutor("Unittest:testtable:insert")
                .SetParameterValue("columnChar", param.ColumnChar)
                .SetParameterValue("columnInt", param.ColumnInt)
                //cmd.SetParameterValue("columnInt", param.columnBool)
                .SetParameterValue("columnDecimal", param.ColumnDecimal)
                .SetParameterValue("columnBool", param.ColumnBool)
                .SetParameterValue("columnDate", param.ColumnDatetime)
                .SetParameterValue("columnDatetime", now);

            var result = await cmd.ExecuteEntityAsync<TestTable_Entity>();
            Assert.NotNull(result);
            Assert.Equal(param.ColumnChar, result.ColumnChar);
            Assert.Equal(param.ColumnInt, result.ColumnInt);
            Assert.Equal(param.ColumnDecimal, result.ColumnDecimal);
            Assert.Equal(param.ColumnBool, result.ColumnBool);
            Assert.Equal(param.ColumnDate.Value.Year, result.ColumnDate.Value.Year);
            Assert.Equal(param.ColumnDate.Value.Month, result.ColumnDate.Value.Month);
            Assert.Equal(param.ColumnDate.Value.Day, result.ColumnDate.Value.Day);
            //Assert.Equal(param.DolumnDatetime.Truncate(TimeSpan.FromSeconds(1)), result.DolumnDatetime.Truncate(TimeSpan.FromSeconds(1)));
        }

        [Fact(Skip = "Won't test dal service")]
        public async Task InsertTestSnakeTable_ReturnEntity_Test()
        {
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var mapper = ComponentMgr.Instance.TryResolve<IMapperMgr>();
            Assert.NotNull(mapper);

            var now = DateTime.UtcNow;
            var ran = new Random().Next(1, 100);
            var param = new TestSnakeTable_Entity()
            {
                column_char = $"{ran}-這是中文",
                column_int = ran,
                column_decimal = 1.1M,
                column_bool = ran % 2 == 1,
                column_date = DateTime.UtcNow,
                column_datetime = now
            };

            var cmd = new CommandExecutor("Unittest:testsnaketable:insert")
                .SetParameterValue("columnChar", param.column_char)
                .SetParameterValue("columnInt", param.column_int)
                //.SetParameterValue("columnInt", param.columnBool)
                .SetParameterValue("columnDecimal", param.column_decimal)
                .SetParameterValue("columnBool", param.column_bool)
                .SetParameterValue("columnDate", param.column_datetime)
                .SetParameterValue("columnDatetime", now);

            var raw = await cmd.ExecuteEntityAsync<TestSnakeTable_Entity>();
            Assert.NotNull(raw);
            Assert.Equal(param.column_char, raw.column_char);
            Assert.Equal(param.column_int, raw.column_int);
            Assert.Equal(param.column_decimal, raw.column_decimal);
            Assert.Equal(param.column_bool, raw.column_bool);
            Assert.Equal(param.column_date.Value.Year, raw.column_date.Value.Year);
            Assert.Equal(param.column_date.Value.Month, raw.column_date.Value.Month);
            Assert.Equal(param.column_date.Value.Day, raw.column_date.Value.Day);

            var result = mapper.ConvertTo<TestTable_Entity>(raw);
            Assert.NotNull(result);
            Assert.Equal(param.column_char, result.ColumnChar);
            Assert.Equal(param.column_int, result.ColumnInt);
            Assert.Equal(param.column_decimal, result.ColumnDecimal);
            Assert.Equal(param.column_bool, result.ColumnBool);
            Assert.Equal(param.column_date.Value.Year, result.ColumnDate.Value.Year);
            Assert.Equal(param.column_date.Value.Month, result.ColumnDate.Value.Month);
            Assert.Equal(param.column_date.Value.Day, result.ColumnDate.Value.Day);
            //Assert.Equal(param.DolumnDatetime.Truncate(TimeSpan.FromSeconds(1)), result.DolumnDatetime.Truncate(TimeSpan.FromSeconds(1)));
        }
    }
}
