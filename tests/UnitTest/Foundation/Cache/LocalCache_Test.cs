using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Caching.Common.Extensions;
using Nwpie.Foundation.Common.Cache.Measurement;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Cache
{
    public class LocalCache_Test : CacheTestBase
    {
        public LocalCache_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task LocalCache_Regression_Test()
        {
            ICache local1 = ComponentMgr.Instance.TryResolve<ILocalCache>();
            Assert.NotNull(local1);
            ICache local2 = ComponentMgr.Instance.TryResolve<ILocalCache>();
            Assert.Same(local1, local2);

            await Execute(local1);
        }

        protected async Task Execute(ICache cache)
        {
            var testData = CreateFakeData(CacheProviderEnum.Local.ToString());

            //cache.IsAttachPrefixkeyEnabled = false;
            var setResult = await cache.SetAsync($"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                testData.First(),
                10
            );
            Assert.True(setResult.IsSuccess);

            var getResult = await cache.GetAsync<TestTable_Entity>(
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}"
            );
            Assert.True(getResult.IsSuccess);
            Assert.NotNull(getResult.Data);
            Assert.Equal(testData.First().ColumnChar, getResult.Data.ColumnChar);
            Assert.Equal(testData.First().ColumnDate, getResult.Data.ColumnDate);
            if (m_AssertSlowPerfEnabled)
            {
                Assert.False(getResult.IsSlowResponse(out var ms),
                    $"Expect {ICacheExtension.MinimumMillisecondsDuration} ms but get {ms}"
                );
            }

            foreach (var item in testData.Skip(1))
            {
                var _ = await cache.SetAsync($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}",
                    item,
                    10
                );
                Assert.True(_.IsSuccess);
            }

            var getResultList = await cache.GetAsync<TestTable_Entity>(new List<string>(){
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            });
            Assert.Equal(2, getResultList.Count());
            Assert.Equal(2, getResultList.Count(o => o.Value.IsSuccess));
            if (m_AssertSlowPerfEnabled)
            {
                Assert.DoesNotContain(getResultList.Values, o => o.IsSlowResponse());
            }

            var rmList = await cache.RemoveAsync(new List<string>(){
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            });
            Assert.Equal(testData.Count() - 2, rmList.Count());
            Assert.Equal(2, rmList.Count(o => o.Value.IsSuccess));

            var rmPrefix = await cache.RemovePatternAsync($"{Utility.GetCallerFullName()}.Model.*");
            Assert.Equal(testData.Count() - 2, rmPrefix.Count());
            Assert.Equal(testData.Count() - 2, rmPrefix.Count(o => o.Value.IsSuccess));

            foreach (var item in testData)
            {
                Assert.False(await cache.ExistsAsync($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}"));

                var _ = await cache.GetAsync<TestTable_Entity>($"{Utility.GetCallerFullName()}.Model");
                Assert.True(_.IsSuccess);
                Assert.Null(_.Data);
            }
        }

        protected bool m_AssertSlowPerfEnabled = true;
    }
}
