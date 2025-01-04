using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.xUnit.App_Start;
using Nwpie.xUnit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.Config
{
    public class ConfigAssets_Test : TestBase
    {
        public ConfigAssets_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Get_Test()
        {
            var apikeyList = DefaultConfigUtil.GetKeyValueListInSection("sdk.api.list",
                new Regex("^asset.service.[a-z]+$"))?.ToList();
            Assert.NotEmpty(apikeyList);

            var configService = DefaultConfigServer;
            var listConfigInfo = m_Configs.Select(o => new ConfigItem()
            {
                ConfigKey = o.Key
            }).ToList();

            // All          => for env in TodoApiKeyList
            // Production   => for env in TodoApiKeyList.TakeProduction()
            // Development  => for env in TodoApiKeyList.TakeNonProduction()
            foreach (var env in apikeyList.TakeProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                try
                {
                    var response = await configService.GetLatest(
                        configs: listConfigInfo,
                        apiName: $"asset.service.{env.Key}",
                        apiKey: env.Value
                    );
                    Assert.NotNull(response);
                    Assert.True(response.IsSuccess);
                    Assert.NotNull(response.Data);
                    Assert.Equal(listConfigInfo.Count(), response.Data.Count());
                    Assert.DoesNotContain(response.Data, o => string.IsNullOrWhiteSpace(o.Value));
                }
                catch (Exception ex)
                {
                    Assert.Null(ex);
                }
            }

            await Task.CompletedTask;
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Set_Test()
        {
            var apikeyList = DefaultConfigUtil.GetKeyValueListInSection("sdk.api.list",
                new Regex("^asset.service.[a-z]+$"))?.ToList();
            Assert.NotEmpty(apikeyList);

            var configService = DefaultConfigServer;
            var encrypt = false;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                foreach (var item in m_Configs)
                {
                    var configKey = item.Key;
                    var configVal = item.Value;
                    Assert.NotNull(configVal);

                    try
                    {
                        var result = await configService.Upsert(configKey: configKey,
                            config: configVal,
                            apiName: $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}",
                            apiKey: env.Value,
                            encrypt: encrypt
                        );
                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.True(result.Data);
                    }
                    catch (Exception ex)
                    {
                        Assert.Null(ex);
                    }
                }
            }

            await Task.CompletedTask;
        }


        [Fact(Skip = "Won't test remote config server")]
        public async Task SetPermission_Test()
        {
            var sdkPrefix = CommonConst.SdkPrefix.ToLower();
            //var behavior = new ConfigPermission()
            //{
            //    AllowRead_Behavior = EMSecurityLevel.Any,
            //    AllowWrite_Behavior = EMSecurityLevel.Specific,
            //    AllowWrite_ApiNames = new List<string>()
            //    {
            //        $"{sdkPrefix}.admin.base",
            //        $"{sdkPrefix}.admin.debug",
            //        $"{sdkPrefix}.admin.dev",
            //        $"{sdkPrefix}.admin.stage",
            //        $"{sdkPrefix}.admin.preprod",
            //        $"{sdkPrefix}.admin.prod"
            //    }
            //};

            var configService = DefaultConfigServer;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeProduction())
            {
                if (EnvironmentEnum.Debug == Enum<EnvironmentEnum>.ParseFromDisplayAttr(env.Key))
                {
                    continue;
                }

                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                foreach (var item in m_Configs)
                {
                    var confiKey = item.Key;
                    try
                    {
                        //var isSuccess = await configService.PermissionModify(configKey: confiKey,
                        //    configPermission: behavior,
                        //    apiName: $"{sdkPrefix}.admin.{env.Key}",
                        //    apiKey: env.Value,
                        //    configServerHost: configserverUrl
                        //);
                        //Assert.True(isSuccess);
                    }
                    catch (Exception ex)
                    {
                        Assert.Null(ex);
                    }
                }
            }

            await Task.CompletedTask;
        }

        public override Task<bool> IsReady()
        {
            var awsCloudWatch_Cfg = new SvcAssets_AwsCfg
            {
                Region = RegionEndpoint.USWest2.SystemName,
                AccessKey = "**",
                SecretKey = "**",
            };

            var awsS3_Cfg = new AwsS3_Option()
            {
                Region = RegionEndpoint.USWest2.SystemName,
                BucketName = "ds1.fruit.prod",
                CachePreSignedUrlEnabled = true,
                CachePreSignedUrlMinutes = 1440,
                AccessKey = "**",
                SecretKey = "**",
                _ts = DateTime.UtcNow
            };

            m_Configs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "foundation.aws.cloudwatch.credential.assets.frontend.service", Serializer.Serialize(awsCloudWatch_Cfg) },
                { "foundation.aws.s3.credential.assets.fakeda", Serializer.Serialize(awsS3_Cfg) }
            };

            return base.IsReady();
        }

        protected Dictionary<string, string> m_Configs;
    }
}
