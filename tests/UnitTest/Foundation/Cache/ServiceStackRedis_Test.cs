using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Common.Cache.Measurement;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.xUnit.Models;
using ServiceStack.Redis;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Cache
{
    public class ServiceStackRedis_Test : CacheTestBase
    {
        public ServiceStackRedis_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        //[Fact(Skip = "Make sure you have Redis on localhost")]
        public async Task RedisCache_Regression_Test()
        {
            string connStr = "localhost:6379";
            connStr = m_RedisConnStr.Split(',')[0];
            IRedisClientsManager clientsManager = new PooledRedisClientManager(connStr)
            {
                ConnectTimeout = 5000
            };

            using (var redis = clientsManager.GetClient())
            {
                await Execute(clientsManager, redis);
            }
        }

        protected async Task Execute(IRedisClientsManager clientsManager, IRedisClient cache)
        {
            var testData = CreateFakeData(CacheProviderEnum.Redis.ToString());

            cache.Set($"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}", testData.First(), TimeSpan.FromSeconds(10));
            var getResult = cache.Get<TestTable_Entity>($"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}");
            Assert.NotNull(getResult);
            Assert.Equal(testData.First().ColumnChar, getResult.ColumnChar);
            Assert.Equal(testData.First().ColumnDate.Value.Date, getResult.ColumnDate.Value.Date);

            foreach (var item in testData.Skip(1))
            {
                var isSuccess = cache.Set($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}",
                    item,
                    TimeSpan.FromSeconds(10)
                );
                Assert.True(isSuccess);
            }

            var getResultList = cache.GetAll<TestTable_Entity>(new List<string>(){
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            });
            Assert.Equal(2, getResultList.Count());

            cache.RemoveAll(new List<string> {
                $"{Utility.GetCallerFullName()}.Model.{testData.First().ColumnInt}",
                $"{Utility.GetCallerFullName()}.Model.{testData.Last().ColumnInt}"
            });

            cache.RemoveByPattern($"{Utility.GetCallerFullName()}.Model.*");

            foreach (var item in testData)
            {
                Assert.False(cache.ContainsKey($"{Utility.GetCallerFullName()}.Model.{item.ColumnInt}"));

                var entity = cache.Get<TestTable_Entity>($"{Utility.GetCallerFullName()}.Model");
                Assert.Null(entity);
            }

            await Task.CompletedTask;
        }

        protected bool m_AssertSlowPerfEnabled = true;
    }
}
