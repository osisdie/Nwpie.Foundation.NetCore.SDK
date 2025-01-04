using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Common.Entities.Application;
using Nwpie.MiniSite.KVS.Common.Entities.Permission;
using Nwpie.MiniSite.KVS.Common.Utilities;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.Contract.Configserver.Permission;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.GetDirectly.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;
using Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Interfaces;
using Nwpie.xUnit.Resources;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.ConfigV2
{
    public class ConfigKVS_Test : TestBase
    {
        public ConfigKVS_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config server")]
        public async Task AllInOne_Integration_Test()
        {
            EncryptConfig_Test();
            await GetConfig_Integration_Test();
            await SetPermission_Integration_Test();
            await SetConfig_Integration_Test();
            await GetDirectly_Integration_Test();
            await SetDirectly_Integration_Test();
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_Migration_Test()
        {
            EncryptConfig_Test();
            await ViaV2_GetPlatformConfig_NotExist_Test();
            await ViaV2_SetPlatformConfig_Test();
            await ViaV2_SetPermission_PlatformConfig_Test();
            await ViaV2_GetPlatformConfig_Test();
        }

        [Fact]
        public void EncryptConfig_Test()
        {
            {
                var rawdata = "hi~";
                var encrypted = CryptoUtils.EncodeToAES(rawdata, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV);
                var decrypted = CryptoUtils.DecodeFromAES(encrypted, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV);
                Assert.Equal(rawdata, decrypted);
            }

            {
                var rawdata = "hi~ apple is eating nwpie";
                var encrypted = CryptoUtils.EncodeToAES(rawdata, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV);
                var decrypted = CryptoUtils.DecodeFromAES(encrypted, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV);
                Assert.Equal(rawdata, decrypted);
            }
        }

        [Fact(Skip = "Use AllInOne_Integration_Test")]
        public async Task GetDirectly_Integration_Test()
        {
            // get nwpie ok
            {
                var service = ComponentMgr.Instance.TryResolve<IKvsGetDirectly_DomainService>();
                service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                service.Headers.Add(ConfigConst.ConfigKey, "dropme.Nwpie.say");
                service.Headers.Add(ConfigConst.Version, ConfigConst.LatestVersion);
                var result = await service.Execute(new KvsGet_ParamModel()
                {
                    ConfigKeys = new List<KvsGet_RequestModelItem>()
                    {
                        new KvsGet_RequestModelItem()
                        {
                            ConfigKey = service.Headers[ConfigConst.ConfigKey],
                            Version = service.Headers[ConfigConst.Version]
                        }
                    }
                });
                Assert.NotNull(result);
                Assert.NotEmpty(result?.RawData);
                Assert.Equal("hi~", result.RawData.First().Value);
            }
        }

        [Fact(Skip = "Use AllInOne_Integration_Test")]
        public async Task SetDirectly_Integration_Test()
        {
            // set
            {
                var service = ComponentMgr.Instance.TryResolve<IKvsSetDirectly_DomainService>();
                service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.base");
                service.Headers[ConfigConst.ConfigKey] = "dropme.cherry.say";
                service.Headers[ConfigConst.Version] = ConfigConst.LatestVersion;
                service.Headers[ConfigConst.RawData] = "hi~ I\"m cherry";
                service.Headers[ConfigConst.NeedEncrypt] = false.ToString();
                var result = await service?.Execute(new KvsSet_ParamModel()
                {
                    ConfigKey = service?.Headers?[ConfigConst.ConfigKey],
                    VersionDisplay = service?.Headers?[ConfigConst.Version],
                    RawData = service?.Headers?[ConfigConst.Version],
                    NeedEncrypt = (service?.Headers?[ConfigConst.NeedEncrypt]).ToBool(),
                });
                Assert.NotNull(result);
                Assert.Equal(new Version(1, 0, 0).ToString(), result.VersionDisplay);
            }
        }

        [Fact(Skip = "Use AllInOne_Integration_Test")]
        public async Task GetConfig_Integration_Test()
        {
            // apple
            {
                // get nwpie ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }

                // get nwpie + overwrite ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ apple is eating nwpie", result.RawData.First().Value);
                }

                // get self app ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.apple.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple", result.RawData.First().Value);
                }

                // get self app ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.apple.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple", result.RawData.First().Value);
                }
            }

            // banana
            {
                // get nwpie ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.banana.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }

                // get nwpie ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.banana.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }

                // get self app ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.banana.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m banana", result.RawData.First().Value);
                }

                // get self app + overwrite ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.banana.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m banana and eating", result.RawData.First().Value);
                }
            }

            // cherry
            {
                // get nwpie fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // get nwpie fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // get friend app ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                FriendAppName = "dropme.banana",
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m banana", result.RawData.First().Value);
                }

                // get friend app ok
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                FriendAppName = "dropme.banana",
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m banana", result.RawData.First().Value);
                }
            }

            // pie
            {
                // get nwpie fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // get nwpie fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // get friend app fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                FriendAppName = "dropme.banana",
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // get friend app fail
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                FriendAppName = "dropme.banana",
                                ConfigKey = "dropme.banana.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }
            }
        }

        [Fact(Skip = "Use AllInOne_Integration_Test")]
        public async Task SetConfig_Integration_Test()
        {
            var sdkPrefix = CommonConst.SdkPrefix.ToLower();
            // apple (NOT allow to set nwpie)
            {
                // set
                try
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = "dropme.Nwpie.say",
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = "hi~ apple is eating nwpie2",
                        NeedEncrypt = true,
                    });
                    Assert.Null(result);
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // apple (debug allow to set nwpie)
            {
                // get previous
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ apple is eating nwpie", result.RawData.First().Value);
                }

                // set
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.debug");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = "dropme.Nwpie.say",
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = "hi~ apple is eating nwpie2",
                        NeedEncrypt = true,
                    });
                    Assert.NotNull(result);
                    Assert.Equal("1.0.1", result.VersionDisplay);
                }

                // get after
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ apple is eating nwpie2", result.RawData.First().Value);
                }
            }

            // apple (allow to set apple)
            {
                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.apple.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple", result.RawData.First().Value);
                }

                // set
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = "dropme.apple.say",
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = "hi~ I\"m apple2",
                    });
                    Assert.NotNull(result);
                    Assert.Equal("1.0.1", result.VersionDisplay);
                }

                // set
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = "dropme.apple.say",
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = "hi~ I\"m apple3",
                    });
                    Assert.NotNull(result);
                    Assert.Equal("1.0.2", result.VersionDisplay);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.apple.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple3", result.RawData.First().Value);
                }
            }

            // nwpie (allow to set nwpie)
            {
                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{sdkPrefix}.admin.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }

                // set
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{sdkPrefix}.admin.debug");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = "dropme.Nwpie.say",
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = "hi~ nwpie2"
                    });
                    Assert.NotNull(result);
                    Assert.Equal(new Version(1, 0, 1).ToString(), result.VersionDisplay);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{sdkPrefix}.admin.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ nwpie2", result.RawData.First().Value);
                }
            }
        }

        [Fact(Skip = "Use AllInOne_Integration_Test")]
        public async Task SetPermission_Integration_Test()
        {
            // pie
            {
                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "dropme.Nwpie.say",
                                Version = ConfigConst.LatestVersion
                            }
                        }
                    });
                    Assert.Null(result);
                }

                // grant
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsPermission_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{CommonConst.SdkPrefix.ToLower()}.admin.debug");
                    var result = await service
                        .Execute(new KvsPermission_ParamModel()
                        {
                            ConfigKey = "dropme.Nwpie.say",
                            Permission = new KvsPermission_RequestModelItem()
                            {
                                AllowRead_Behavior = AccessLevelEnum.Specific,
                                AllowRead_ApiNames = new List<string>() { "dropme.apikey.cherry.pie.dev" }
                            }
                        });

                    Assert.NotNull(result);
                    Assert.True(result.CountInserted > 0 || result.CountUpdated > 0);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                                new KvsGet_RequestModelItem()
                                {
                                    ConfigKey = "dropme.Nwpie.say",
                                    Version = ConfigConst.LatestVersion
                                }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                                new KvsGet_RequestModelItem()
                                {
                                    ConfigKey = "dropme.Nwpie.say",
                                    Version = ConfigConst.LatestVersion
                                }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~", result.RawData.First().Value);
                }
            }

            {
                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                                new KvsGet_RequestModelItem()
                                {
                                    FriendAppName = "dropme.apple",
                                    ConfigKey = "dropme.apple.say",
                                    Version = ConfigConst.LatestVersion
                                }
                        }
                    });
                    Assert.Null(result);
                }

                // grant
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsPermission_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.apple.dev");
                    var result = await service.Execute(new KvsPermission_ParamModel()
                    {
                        FriendAppName = "dropme.apple",
                        ConfigKey = "dropme.apple.say",
                        Permission = new KvsPermission_RequestModelItem()
                        {
                            AllowRead_Behavior = AccessLevelEnum.Specific,
                            AllowRead_ApiNames = new List<string>() { "dropme.apikey.cherry.pie.dev" }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.True(result.CountInserted > 0 || result.CountUpdated > 0);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.dev");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                                new KvsGet_RequestModelItem()
                                {
                                    FriendAppName = "dropme.apple",
                                    ConfigKey = "dropme.apple.say",
                                    Version = ConfigConst.LatestVersion
                                }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple", result.RawData.First().Value);
                }

                // get
                {
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, "dropme.apikey.cherry.pie.debug");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                                new KvsGet_RequestModelItem()
                                {
                                    FriendAppName = "dropme.apple",
                                    ConfigKey = "dropme.apple.say",
                                    Version = ConfigConst.LatestVersion
                                }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.Equal("hi~ I\"m apple", result.RawData.First().Value);
                }
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_SetSingleConfig_Test()
        {
            var sdkPrefix = CommonConst.SdkPrefix.ToLower();
            var encrypt = false;
            var env = AdminApiKeyList.First();
            var baseServiceHostUrl = ConfigServiceUrlMap[env.Key];
            Assert.NotNull(baseServiceHostUrl);

            var configKey = SysConfigKey.Default_Notification_HostUrl_ConfigKey;
            var configValue = GetMapValueDefault(configKey, env.Key);

            // set nwpie ok
            {
                var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                service.Headers.Add(CommonConst.ApiName, $"{sdkPrefix}.admin.{env.Key}");
                var result = await service.Execute(new KvsSet_ParamModel()
                {
                    ConfigKey = configKey,
                    VersionDisplay = ConfigConst.LatestVersion,
                    RawData = configValue,
                    NeedEncrypt = encrypt,
                });
                Assert.NotNull(result);
                Assert.NotNull(result.VersionDisplay);
            }

            // get nwpie ok
            {
                var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                service.Headers.Add(CommonConst.ApiName, $"{sdkPrefix}.admin.{env.Key}");
                var result = await service.Execute(new KvsGet_ParamModel()
                {
                    ConfigKeys = new List<KvsGet_RequestModelItem>()
                    {
                        new KvsGet_RequestModelItem()
                        {
                            ConfigKey = configKey,
                            Version = ConfigConst.LatestVersion
                        }
                    }
                });
                Assert.NotNull(result);
                Assert.NotEmpty(result?.RawData);
                Assert.Equal(configValue, result.RawData.First().Value);
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_SetPlatformConfig_Test()
        {
            var email_template = Resource1.mail_template;
            Assert.False(string.IsNullOrWhiteSpace(email_template));

            var encrypt = false;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            // Inheris from stage
            var env = AdminApiKeyList.TakeNonProduction()
                .Where(o => EnvironmentEnum.Staging == Enum<EnvironmentEnum>.ParseFromDisplayAttr(o.Key))
                .FirstOrDefault();

            Assert.NotNull(env.Key);
            Assert.NotNull(env.Value);

            foreach (var item in ConfigKeyForSetForAllEnvMap)
            {
                var configKey = item.Key;
                var configVal = GetMapValueDefault(configKey, env.Key);
                Assert.NotNull(configVal);

                try
                {
                    // set nwpie ok
                    var service = ComponentMgr.Instance.TryResolve<IKvsSet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{CommonConst.SdkPrefix.ToLower()}.admin.base");
                    var result = await service.Execute(new KvsSet_ParamModel()
                    {
                        ConfigKey = configKey,
                        VersionDisplay = ConfigConst.LatestVersion,
                        RawData = configVal,
                        NeedEncrypt = encrypt,
                    });
                    Assert.NotNull(result);
                    Assert.NotNull(result.VersionDisplay);
                }
                catch (Exception ex)
                {
                    Assert.Null(ex);
                }
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_SetPermission_PlatformConfig_Test()
        {
            foreach (var item in ConfigKeyForSetForAllEnvMap)
            {
                var confiKey = item.Key;
                try
                {
                    // grant
                    var service = ComponentMgr.Instance.TryResolve<IKvsPermission_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"{CommonConst.SdkPrefix.ToLower()}.admin.base");
                    var result = await service.Execute(new KvsPermission_ParamModel()
                    {
                        ConfigKey = confiKey,
                        Permission = new KvsPermission_RequestModelItem()
                        {
                            AllowRead_Behavior = AccessLevelEnum.Any,
                            //AllowRead_ApiNames = new List<string>() { ""todo.base","acct.base","ntfy.base","ntfy.job.base"" }
                        }
                    });
                    Assert.NotNull(result);
                    Assert.True(result.CountInserted > 0 || result.CountUpdated > 0);
                }
                catch (Exception ex)
                {
                    Assert.Null(ex);
                }
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_GetPlatformConfig_NotExist_Test()
        {
            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeNonProduction())
            {
                try
                {
                    // get nwpie fail
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"todo.{env.Key}");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = new List<KvsGet_RequestModelItem>()
                        {
                            new KvsGet_RequestModelItem()
                            {
                                ConfigKey = "You cannot pass"
                            }
                        }
                    });
                    Assert.Null(result);
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task ViaV2_GetPlatformConfig_Test()
        {
            var listConfig = ConfigKeyForReadList.Select(o => new KvsGet_RequestModelItem()
            {
                ConfigKey = o.ConfigKey,
                Version = o.Version ?? ConfigConst.LatestVersion
            })?.ToList();
            Assert.NotEmpty(listConfig);

            // All          => for env in TodoApiKeyList
            // Production   => for env in TodoApiKeyList.TakeProduction()
            // Development  => for env in TodoApiKeyList.TakeNonProduction()
            foreach (var env in TodoApiKeyList.TakeNonProduction())
            {
                try
                {
                    // get nwpie ok
                    var service = ComponentMgr.Instance.TryResolve<IKvsGet_DomainService>();
                    service.Headers.Add(CommonConst.ApiName, $"todo.{env.Key}");
                    var result = await service.Execute(new KvsGet_ParamModel()
                    {
                        ConfigKeys = listConfig
                    });
                    Assert.NotNull(result);
                    Assert.NotEmpty(result?.RawData);
                    Assert.DoesNotContain(result.RawData, o => string.IsNullOrWhiteSpace(o.Value));
                }
                catch (Exception ex)
                {
                    Assert.Null(ex);
                }
            }
        }

        public override async Task<bool> IsReady()
        {
            Assert.NotEmpty(AdminApiKeyList);
            Assert.NotEmpty(ConfigKeyForSetForAllEnvMap);
            Assert.NotEmpty(ConfigServiceUrlMap);

            {
                var cmd = new CommandExecutor("Unittest:Kvs:Truncate");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:Application:Apple");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:Application:Banana");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:Application:Cherry");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:Configkey");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:ConfigValues");
                var count = await cmd.ExecuteScalarAsync<int>();
            }
            {
                var cmd = new CommandExecutor("Unittest:Kvs:Initialize:Permission");
                var count = await cmd.ExecuteScalarAsync<int>();
            }

            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    m_ApplicationList = await ApplicationDomainHelper.ListApplicationAsync();
                }),

                Task.Run(async () =>
                {
                    m_PermissionList = await PermissionDomainHelper.ListPermissionAsync();
                }),

                Task.Run(async () =>
                {
                    m_ApikeyList = await ApiKeyDomainHelper.ListApiKeyAsync();
                }),

                Task.Run(async () =>
                {
                    m_ConfigValueList = await ConfigDomainHelper.ListConfigValueAsync();
                }),

                Task.Run(async () =>
                {
                    m_PermissionApikeyList = await ConfigDomainHelper.ListConfigPermissionAsync();
                })
            };

            Task.WaitAll(tasks.ToArray());

            Assert.NotEmpty(m_ApplicationList);
            Assert.NotEmpty(m_PermissionList);
            Assert.NotEmpty(m_ApikeyList);
            Assert.NotEmpty(m_ConfigValueList);
            Assert.NotEmpty(m_PermissionApikeyList);

            return true;
        }

        protected List<APPLICATION_Entity> m_ApplicationList;
        protected List<PERMISSION_Entity> m_PermissionList;
        protected List<API_KEY_Entity> m_ApikeyList;
        protected List<CONFIG_VALUES_Entity> m_ConfigValueList;
        protected List<PERMISSION_CONFIG_KEY_Entity> m_PermissionApikeyList;
    }
}
