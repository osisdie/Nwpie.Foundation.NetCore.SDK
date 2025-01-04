using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Cache.Measurement;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Cache
{
    public abstract class CacheTestBase : TestBase
    {
        public CacheTestBase(ITestOutputHelper output) : base(output) { }

        protected List<TestTable_Entity> CreateFakeData(string columnChar)
        {
            return new List<TestTable_Entity>()
            {
                new TestTable_Entity()
                {
                    ColumnInt = 1,
                    ColumnChar = columnChar,
                    ColumnDate = DateTime.UtcNow
                },
                new TestTable_Entity()
                {
                    ColumnInt = 2,
                    ColumnChar = columnChar,
                    ColumnDate = DateTime.UtcNow
                },
                new TestTable_Entity()
                {
                    ColumnInt = 3,
                    ColumnChar = columnChar,
                    ColumnDate = DateTime.UtcNow
                },
                new TestTable_Entity()
                {
                    ColumnInt = 4,
                    ColumnChar = columnChar,
                    ColumnDate = DateTime.UtcNow//.Truncate(TimeSpan.FromSeconds(1)).Value.ToUniversalTime()
                },
            };
        }

        public override Task<bool> IsReady()
        {
            CacheMeasurementExtension.RemoteLoggingEnabled = false;
            CacheMeasurementExtension.MeasurementEnabled = false;

            m_RedisConnStr = ServiceContext.Configuration?[SysConfigKey.Default_AWS_Redis_ConnectionString_ConfigKey];
            Assert.NotNull(m_RedisConnStr);

            return base.IsReady();
        }

        protected string m_RedisConnStr;
    }
}
