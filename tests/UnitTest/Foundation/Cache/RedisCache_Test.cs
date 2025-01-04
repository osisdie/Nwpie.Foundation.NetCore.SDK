using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Caching.Common.Extensions;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Cache.Measurement;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.xUnit.Models;
using RabbitMQ.Client;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Cache
{
    public class RedisCache_Test : CacheTestBase
    {
        public RedisCache_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void RedisCache_FailOver_Test()
        {
            var conn = SysConfigKey
                .Default_AWS_Redis_ConnectionString_ConfigKey
                .ConfigServerRawValue();
            Assert.NotNull(conn);

            ICache redis1 = ComponentMgr.Instance.TryResolve<IRedisCache>();
            if (null == redis1)
            {
                redis1 = ComponentMgr.Instance.GetCache<IRedisCache>(isHealthCheck: false, isFailOverToLocalCache: false);
                Assert.Null(redis1);

                // auto fix
                redis1 = ComponentMgr.Instance.GetCache<IRedisCache>(isHealthCheck: true, isFailOverToLocalCache: true);
                Assert.NotNull(redis1);

                var isHealthy = CacheUtils.IsHealthy(redis1);
                Assert.True(isHealthy);
            }
            else
            {
                var isHealthy = CacheUtils.IsHealthy(redis1);
                if (false == isHealthy)
                {
                    // auto fix
                    redis1 = ComponentMgr.Instance.GetCache<IRedisCache>(isHealthCheck: true, isFailOverToLocalCache: true);
                    Assert.NotNull(redis1);

                    isHealthy = CacheUtils.IsHealthy(redis1);
                    Assert.True(isHealthy);
                }
            }
        }

        [Fact]
        //[Fact(Skip = "Make sure you have Redis on localhost")]
        public async Task RedisCache_Regression_Test()
        {
            ICache redis1 = ComponentMgr.Instance.TryResolve<IRedisCache>();
            Assert.NotNull(redis1);
            ICache redis2 = ComponentMgr.Instance.TryResolve<IRedisCache>();
            Assert.Equal(redis1._id, redis2._id);

            // local test
            //{
            //    var localConfig = new RedisCache_Option()
            //    {
            //        ClientName = "(local)",
            //        Host = "localhost",
            //        Port = 6379,
            //        DbIndex = 0,
            //        ConnectTimeout = 5000,
            //        SyncTimeout = 5000,
            //        ConnectionString = "localhost:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0"
            //    };
            //    redis1 = new RedisCache(new ConfigOptions<RedisCache_Option>(localConfig));
            //    var isHealthy = CacheUtils.IsHealthy(redis1);
            //    Assert.True(isHealthy);
            //}

            await Execute(redis1);
        }

        protected async Task Execute(ICache cache)
        {
            var testData = CreateFakeData(CacheProviderEnum.Redis.ToString());

            //cache.IsAttachPrefixkeyEnabled = false;
            var setResult = await cache.SetAsync($"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                testData.First(),
                10
            );
            Assert.True(setResult.IsSuccess);

            var getResult = await cache.GetAsync<TestTable_Entity>($"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}");
            Assert.True(getResult.IsSuccess);
            Assert.NotNull(getResult.Data);
            Assert.Equal(testData.First().ColumnChar, getResult.Data.ColumnChar);
            Assert.Equal(testData.First().ColumnDate, getResult.Data.ColumnDate);
            if (m_AssertSlowPerfEnabled)
            {
                Assert.False(getResult.IsSlowResponse(out var ms), $"Expect {ICacheExtension.MinimumMillisecondsDuration} ms but get {ms}");
            }

            foreach (var item in testData.Skip(1))
            {
                var _ = await cache.SetAsync($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}",
                    item,
                    10
                );
                Assert.True(_.IsSuccess);
            }

            var getResultList = await cache.GetAsync<TestTable_Entity>([
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            ]);
            Assert.Equal(2, getResultList.Count);
            Assert.Equal(2, getResultList.Count(o => o.Value.IsSuccess));
            if (m_AssertSlowPerfEnabled)
            {
                Assert.DoesNotContain(getResultList.Values, o => o.IsSlowResponse());
            }

            var rmList = await cache.RemoveAsync([
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            ]);
            Assert.Equal(2, rmList.Count);
            Assert.Equal(2, rmList.Count(o => o.Value.IsSuccess));

            var rmPrefix = await cache.RemovePatternAsync($"{Utility.GetCallerFullName()}.Model.*");
            Assert.Equal(testData.Count - 2, rmPrefix.Count);
            Assert.Equal(testData.Count - 2, rmPrefix.Count(o => o.Value.IsSuccess));

            foreach (var item in testData)
            {
                Assert.False(await cache.ExistsAsync($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}"));
                var _ = await cache.GetAsync<TestTable_Entity>($"{Utility.GetCallerFullName()}.Model");
                Assert.True(_.IsSuccess);
                Assert.Null(_.Data);
            }
        }

        [Fact(Skip = "sudo service redis-server status")]
        public void LocalConnect_Test()
        {
            const string cacheKey = "test_key";
            const string cacheValue = "test_value";

            try
            {
                var connStr = "localhost:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0,abortConnect=false";
                //var connStr = "127.0.0.1:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0,abortConnect=false";
                //var connStr = "172.18.140.253:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0,abortConnect=false";

                var m_Multiplexer = ConnectionMultiplexer.Connect(connStr);
                var db = m_Multiplexer.GetDatabase();

                Console.WriteLine("Connected to Redis！");

                db.StringSet(cacheKey, cacheValue);
                var retrievedValue = db.StringGet(cacheKey);
                Assert.True(retrievedValue.HasValue);

                db.KeyDelete(cacheKey);
                var deletedValue = db.StringGet(cacheKey);
                Assert.False(deletedValue.HasValue);
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        protected bool m_AssertSlowPerfEnabled = false;
    }
}
