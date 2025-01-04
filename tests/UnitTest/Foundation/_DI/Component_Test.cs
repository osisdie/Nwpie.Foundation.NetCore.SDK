using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation._DI
{
    public class Component_Test : TestBase
    {
        public Component_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Component_Map_Test()
        {
            var map = new ConcurrentDictionary<Type, object>();
            var defaultCache = new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                TrackStatistics = true
            }));
            var cache1 = map.GetOrAdd(typeof(ILocalCache), defaultCache);
            Assert.Same(defaultCache, cache1);

            var cache2 = map.GetOrAdd(typeof(ILocalCache), (T) =>
            {
                return new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
                {
                    TrackStatistics = true
                }));
            });
            Assert.Same(defaultCache, cache2);
        }

        [Fact]
        public async Task Component_Resolve_Test()
        {
            var logger = LogMgr.CreateLogger(GetType());
            Assert.NotNull(logger);

            Assert.NotNull(ServiceContext.Configuration);
            Assert.NotNull(ServiceContext.ApiName);
            Assert.NotNull(ServiceContext.ApiKey);
            Assert.NotNull(ServiceContext.SdkEnv);

            //ServiceResponse<IConfigOptions<Platform_Option>> platformCfg = configMgr.GetStartupConfig<Platform_Option>();
            //Assert.True(platformCfg.IsSuccess);
            //Assert.NotNull(platformCfg.Data);

            //ServiceResponse<IConfigOptions<Encrypted_Option>> encryptedCfg = configMgr.Get<Encrypted_Option>("Encrypted.json".ToLower());
            //Assert.True(encryptedCfg.IsSuccess);
            //Assert.NotNull(encryptedCfg.Data);

            //var configServer1 = Client_ConfigServer.Instance;
            //Assert.NotNull(configServer1);
            //var configServer2 = ComponentMgr.Instance.TryResolve<IClient_ConfigServer>();
            //Assert.NotNull(configServer2);
            //Assert.Same(configServer1, configServer2);
            var configServer3 = ComponentMgr.Instance.TryResolve<IConfigClient>();
            Assert.NotNull(configServer3);

            var conn1 = (SysConfigKey
                .PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                .ConfigServerRawValue();
            //var conn1 = ServiceContext.Configuration.GetConnectionString(ConfigConst.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db");
            Assert.NotNull(conn1);

            var serializer = ComponentMgr.Instance.TryResolve<ISerializer>();
            Assert.NotNull(serializer);

            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            Assert.NotNull(defaultSerializer);

            var jsonSerializer = ComponentMgr.Instance.TryResolve<IJsonConfigSerializer>();
            Assert.NotNull(jsonSerializer);

            var httpSerializer = ComponentMgr.Instance.TryResolve<IHttpSerializer>();
            Assert.NotNull(httpSerializer);

            var entitySerializer = ComponentMgr.Instance.TryResolve<IEntitySerializer>();
            Assert.NotNull(entitySerializer);

            var mapperMgr = ComponentMgr.Instance.TryResolve<IMapperMgr>();
            Assert.NotNull(mapperMgr);

            var memoryCache = ComponentMgr.Instance.TryResolve<ICache>();
            Assert.NotNull(memoryCache);

            var localCache = ComponentMgr.Instance.TryResolve<ILocalCache>();
            Assert.NotNull(localCache);

            var redisOption = ComponentMgr.Instance.TryResolve<IConfigOptions<RedisCache_Option>>();
            Assert.NotNull(redisOption);
            Assert.NotNull(redisOption.Value);
            Assert.NotNull(redisOption.Value.ConnectionString);

            var redisCache = ComponentMgr.Instance.TryResolve<IRedisCache>();
            Assert.NotNull(redisCache);

            var defaultCache1 = ComponentMgr.Instance.GetDefaultCache();
            Assert.NotNull(defaultCache1);

            var defaultCache2 = ComponentMgr.Instance.TryResolve<ICache>();
            Assert.NotNull(defaultCache2);
            Assert.Same(defaultCache1, defaultCache2);

            var s3Option = ComponentMgr.Instance.TryResolve<IConfigOptions<AwsS3_Option>>();
            Assert.NotNull(s3Option);
            Assert.NotNull(s3Option.Value);
            Assert.NotNull(s3Option.Value.Region);

            var s3Mgr = ComponentMgr.Instance.TryResolve<IAwsS3Factory>();
            Assert.NotNull(s3Mgr);
            IStorage defaultS3 = ComponentMgr.Instance.TryResolve<IS3StorageClient>();
            Assert.NotNull(defaultS3);

            var notificationClient = ComponentMgr.Instance.TryResolve<INotificationHttpClient>();
            Assert.NotNull(notificationClient);
            var isReady = await notificationClient.IsReady();
            Assert.True(isReady);
        }

        [Fact]
        public void ConfigMgr_Test()
        {
            // Make sure remote ConfigKey/Value exists
            var todoConnDB = (SysConfigKey
                .PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                .ConfigServerRawValue();
            Assert.NotNull(todoConnDB);

            var auth_connDB = (SysConfigKey
                .PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "auth_db")
                .ConfigServerRawValue();
            Assert.NotNull(auth_connDB);

            var connRedis = SysConfigKey
                .Default_AWS_Redis_ConnectionString_ConfigKey
                .ConfigServerRawValue();
            Assert.NotNull(connRedis);

            var sqsOption = SysConfigKey
                .Default_AWS_SQS_Urls_Notification_ConfigKey
                .ConfigServerValue<AwsSQS_Option>();
            Assert.NotNull(sqsOption);
            Assert.NotNull(sqsOption.Region);

            var s3Option = SysConfigKey
                .Default_AWS_S3_Credential_ConfigKey
                .ConfigServerValue<AwsS3_Option>();
            Assert.NotNull(s3Option);
            Assert.NotNull(s3Option.Region);

            var todoS3option = (SysConfigKey
                .PrefixKey_AWS_S3_Credential_ConfigKey + "todo_db")
                .ConfigServerValue<AwsS3_Option>();
            Assert.NotNull(todoS3option);
            Assert.NotNull(todoS3option.Region);
        }
    }
}
