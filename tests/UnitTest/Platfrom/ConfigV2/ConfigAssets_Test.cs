using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.Contract.Configserver.Permission;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;
using Nwpie.xUnit.App_Start;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.ConfigV2
{
    public class ConfigAssets_Test : TestBase
    {
        public ConfigAssets_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Get_Test()
        {
            var app = "asset.service";
            var apikeyList = DefaultConfigUtil.GetKeyValueListInSection($"sdk.api.list",
                new Regex("^asset.service.[a-z]+$"))?.ToList();
            Assert.NotEmpty(apikeyList);

            foreach (var env in apikeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key];
                Assert.NotNull(configserverUrl);

                foreach (var config in m_Configs)
                {
                    // get app ok
                    {
                        var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                        service.Headers.Add(CommonConst.ApiName, $"{app}.{env.Key}");
                        var kvsGet_RequestModelItem = new KvsGet_RequestModelItem()
                        {
                            ConfigKey = config.Key,
                            Version = ConfigConst.LatestVersion
                        };
                        var result = await service.Execute(new KvsGet_ParamModel()
                        {
                            ConfigKeys = new List<KvsGet_RequestModelItem>()
                            {
                                kvsGet_RequestModelItem
                            }
                        });
                        Assert.NotNull(result);
                        Assert.NotEmpty(result?.RawData);
                    }
                }
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Set_Test()
        {
            var app = "asset.service";
            var apikeyList = DefaultConfigUtil.GetKeyValueListInSection($"sdk.api.list",
                new Regex("^asset.service.[a-z]+$"))?.ToList();
            Assert.NotEmpty(apikeyList);

            var encrypt = false;
            foreach (var env in apikeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key];
                Assert.NotNull(configserverUrl);

                foreach (var config in m_Configs)
                {
                    // set app ok
                    {
                        var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                        service.Headers.Add(CommonConst.ApiName, $"{app}.{env.Key}");
                        var result = await service.Execute(new KvsSet_ParamModel()
                        {
                            ConfigKey = config.Key,
                            VersionDisplay = ConfigConst.LatestVersion,
                            RawData = config.Value,
                            NeedEncrypt = encrypt,
                        });
                        Assert.NotNull(result);
                        Assert.NotNull(result.VersionDisplay);
                    }

                    // set permission app ok
                    {
                        var service = ComponentMgr.Instance.TryResolve<IKvsPermission_DomainService>();
                        service.Headers.Add(CommonConst.ApiName, $"{app}.{env.Key}");
                        var result = await service.Execute(new KvsPermission_ParamModel()
                        {
                            ConfigKey = config.Key,
                            Permission = new KvsPermission_RequestModelItem()
                            {
                                AllowRead_Behavior = AccessLevelEnum.Specific,
                                AllowRead_ApiNames = new List<string>() { $"{app}.base", $"{app}.{env.Key}" }
                            }
                        });
                        Assert.NotNull(result);
                        Assert.True(result.CountInserted > 0 || result.CountUpdated > 0);
                    }

                    // get app ok
                    {
                        var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                        service.Headers.Add(CommonConst.ApiName, $"{app}.{env.Key}");
                        var kvsGet_RequestModelItem = new KvsGet_RequestModelItem()
                        {
                            ConfigKey = config.Key,
                            Version = ConfigConst.LatestVersion
                        };
                        var result = await service.Execute(new KvsGet_ParamModel()
                        {
                            ConfigKeys = new List<KvsGet_RequestModelItem>()
                            {
                                kvsGet_RequestModelItem
                            }
                        });
                        Assert.NotNull(result);
                        Assert.NotEmpty(result?.RawData);
                    }
                }

            }
        }

        public override Task<bool> IsReady()
        {
            var awsCloudWatch_Cfg = new AwsCfg
            {
                Region = RegionEndpoint.USWest2.SystemName,
                AccessKey = "**",
                SecretKey = "**",
            };

            var awsS3_Cfg = new AwsS3_Option()
            {
                Region = RegionEndpoint.USWest2.SystemName,
                BucketName = "ds1.fruit.dev",
                CachePreSignedUrlEnabled = true,
                CachePreSignedUrlMinutes = 1440,
                AccessKey = "**",
                SecretKey = "**",
                _ts = DateTime.UtcNow
            };

            m_Configs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"foundation.aws.cloudwatch.credential.assets.frontend.service", Serializer.Serialize(awsCloudWatch_Cfg) },
                {"foundation.aws.s3.credential.assets.fakeda", Serializer.Serialize(awsS3_Cfg) }
            };

            return base.IsReady();
        }

        Dictionary<string, string> m_Configs;
    }

    public class AwsCfg
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public DateTime? _ts { get; set; }
    }
}
