using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.Location.Contract.Config.Refresh;
using Nwpie.xUnit.Resources;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.Config
{
    public class ConfigV1_Test : TestBase
    {
        public ConfigV1_Test(ITestOutputHelper output) : base(output) { }

        #region original config sdk
        [Fact(Skip = "Won't test remote config server")]
        public async Task Set_SingleConfig_ViaOriginalClient_Test()
        {
            var configService = DefaultConfigServer;
            var encrypt = false;
            var env = AdminApiKeyList.First();
            var configserverUrl = ConfigServiceUrlMap[env.Key];
            Assert.NotNull(configserverUrl);

            var configKey = SysConfigKey.Default_Notification_HostUrl_ConfigKey;
            {
                var result = await configService.Upsert(configKey: configKey,
                    config: GetMapValueDefault(configKey, env.Key),
                    apiName: $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}",
                    apiKey: env.Value,
                    encrypt: encrypt
                );
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
            }

            {
                var multipleKeys = new List<ConfigItem>()
                {
                    new()
                    {
                        ConfigKey = configKey
                    },
                };

                var response = await configService.GetLatest(configs: multipleKeys,
                    apiName: $"todo.{env.Key}",
                    apiKey: env.Value
                );
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.Contains(configKey, response.Data);
                Assert.False(string.IsNullOrWhiteSpace(response.Data[configKey]));
            }

            await Task.CompletedTask;
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task Set_PlatformConfig_ViaOriginalClient_Test()
        {
            var email_template = Resource1.mail_template;
            Assert.False(string.IsNullOrWhiteSpace(email_template));

            var configService = DefaultConfigServer;
            var encrypt = false;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                foreach (var item in ConfigKeyForSetForAllEnvMap)
                {
                    if (m_CurrentProcessConfigKeys?.Count > 0 &&
                        false == m_CurrentProcessConfigKeys.Contains(item.Key))
                    {
                        continue;
                    }

                    var configKey = item.Key;
                    var configVal = GetMapValueDefault(configKey, env.Key);
                    Assert.NotNull(configVal);

                    try
                    {
                        var result = await configService.Upsert(configKey: configKey,
                            config: configVal,
                            apiName: $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}",
                            apiKey: env.Value,
                            encrypt: encrypt
                        );
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
        public async Task PermissionModify_PlatformConfig_Test()
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
            foreach (var env in AdminApiKeyList.TakeNonProduction())
            {
                if (EnvironmentEnum.Debug == Enum<EnvironmentEnum>.ParseFromDisplayAttr(env.Key))
                {
                    continue;
                }

                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                foreach (var item in ConfigKeyForSetForAllEnvMap)
                {
                    if (m_CurrentProcessConfigKeys?.Count > 0 &&
                        false == m_CurrentProcessConfigKeys.Contains(item.Key))
                    {
                        continue;
                    }

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

        [Fact(Skip = "Won't test remote config server")]
        public async Task Get_PlatformConfig_ViaOriginalClient_Test()
        {
            var configService = DefaultConfigServer;

            // All          => for env in TodoApiKeyList
            // Production   => for env in TodoApiKeyList.TakeProduction()
            // Development  => for env in TodoApiKeyList.TakeNonProduction()
            foreach (var env in TodoApiKeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                try
                {
                    var listConfigInfos = ConfigKeyForReadList.ConvertAll<ConfigItem>(o => o.ConvertTo<ConfigItem>());
                    var response = await configService.GetLatest(configs: listConfigInfos,
                        apiName: $"todo.{env.Key}",
                        apiKey: env.Value
                    );
                    Assert.NotNull(response);
                    Assert.True(response.IsSuccess);
                    Assert.NotNull(response.Data);
                    Assert.Equal(ConfigKeyForReadList.Count, response.Data.Count);
                    Assert.DoesNotContain(response.Data, o => string.IsNullOrWhiteSpace(o.Value));
                }
                catch (Exception ex)
                {
                    Assert.Null(ex);
                }
            }

            await Task.CompletedTask;
        }

        #endregion

        #region custom config client
        [Fact(Skip = "Won't test remote config server")]
        public async Task Set_SingleConfig_ViaCustomClient_Test()
        {
            var configService = DefaultConfigServer;
            var encrypt = false;
            var env = AdminApiKeyList.First();
            var configserverUrl = ConfigServiceUrlMap[env.Key];
            Assert.NotNull(configserverUrl);

            //configService.GetUrl = configserverUrl.ResolveGetActionUrl();
            //configService.SetUrl = configserverUrl.ResolveSetActionUrl();

            var configKey = SysConfigKey.Default_Notification_HostUrl_ConfigKey;
            var configValue = GetMapValueDefault(configKey, env.Key);

            // set config
            {
                var response = await configService.Upsert(configKey: configKey,
                    config: configValue,
                    encrypt: encrypt
                );
                Assert.True(response.IsSuccess);
            }

            // get config
            {
                var response = await configService.GetLatest(configKey);
                Assert.True(response.IsSuccess, response.ErrMsg);
                Assert.NotNull(response.Data);
                Assert.Equal(configValue, response.Data);
            }
        }

        [Fact(Skip = "TODO")]
        public async Task Set_PlatformConfig_ViaCustomClient_Test()
        {
            var email_template = Resource1.mail_template;
            Assert.False(string.IsNullOrWhiteSpace(email_template));

            var configService = DefaultConfigServer;
            var encrypt = false;

            configService.DefaultTimeoutSecs = DefaultRemoteConfigClient.GlobalTimeoutSecs;
            configService.DefaultRetries = DefaultRemoteConfigClient.GlobalRetries;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                configService.GetUrl = configserverUrl.ResolveGetActionUrl();
                configService.SetUrl = configserverUrl.ResolveSetActionUrl();

                foreach (var item in ConfigKeyForSetForAllEnvMap)
                {
                    if (m_CurrentProcessConfigKeys?.Count > 0 &&
                        false == m_CurrentProcessConfigKeys.Contains(item.Key))
                    {
                        continue;
                    }

                    var confiKey = item.Key;
                    var configVal = GetMapValueDefault(confiKey, env.Key);
                    Assert.NotNull(configVal);

                    var response = await configService.Upsert(configKey: confiKey,
                        config: configVal,
                        encrypt: encrypt,
                        apiName: $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}",
                        apiKey: env.Value
                    );
                    Assert.True(response.IsSuccess, response.ErrMsg);
                }
            }
        }

        [Fact(Skip = "TODO")]
        public async Task RefreshConfigCache_Test()
        {
            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList.TakeNonProduction())
            {
                var locationServerUrl = LocationServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(locationServerUrl);

                var getUrl = locationServerUrl.ResolveGetActionUrl(LocationConst.HttpPathToConfigContractControllerName);
                var refreshUrl = locationServerUrl.ResolveRefreshActionUrl(LocationConst.HttpPathToConfigContractControllerName);
                var request = new LocConfigRefresh_Request
                {
                    Data = new LocConfigRefresh_RequestModel
                    {
                        ConfigKeys = m_CurrentProcessConfigKeys,
                        PullLatest = true,
                    },
                };

                var header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { CommonConst.ApiName, $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}" },
                    { CommonConst.ApiKey, env.Value }
                };

                {
                    var response = await ApiUtils.HttpPost(refreshUrl,
                        Serializer.Serialize(request),
                        header
                    );

                    Assert.True(response.IsSuccess, response.ErrMsg);
                }
            }
        }

        [Fact(Skip = "TODO")]
        public async Task Get_PlatformConfig_NotExist_Test()
        {
            var configService = DefaultConfigServer;
            configService.DefaultTimeoutSecs = ConfigConst.DefaultHttpTimeout * 2;
            configService.DefaultRetries = 0;

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList)
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                configService.GetUrl = configserverUrl.ResolveGetActionUrl();
                configService.SetUrl = configserverUrl.ResolveSetActionUrl();

                {
                    var multipleKeys = new List<ConfigItem>()
                    {
                        new("You cannot pass")
                    };

                    var response = await configService.GetLatest(multipleKeys,
                        apiName: $"{CommonConst.SdkPrefix.ToLower()}.admin.{env.Key}",
                        apiKey: env.Value
                    );
                    Assert.True(response.IsSuccess, response.ErrMsg);
                    Assert.Null(response.Data);
                }
            }
        }

        [Fact(Skip = "TODO")]
        public async Task Get_PlatformConfig_ViaCustomClient_Test()
        {
            var configService = DefaultConfigServer;
            configService.DefaultTimeoutSecs = DefaultRemoteConfigClient.GlobalTimeoutSecs;
            configService.DefaultRetries = DefaultRemoteConfigClient.GlobalRetries;

            // All          => for env in TodoApiKeyList
            // Production   => for env in TodoApiKeyList.TakeProduction()
            // Development  => for env in TodoApiKeyList.TakeNonProduction()
            foreach (var env in TodoApiKeyList.TakeNonProduction())
            {
                var configserverUrl = ConfigServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(configserverUrl);

                configService.GetUrl = configserverUrl.ResolveGetActionUrl();
                configService.SetUrl = configserverUrl.ResolveSetActionUrl();

                {
                    var response = await configService.GetLatest(ConfigKeyForReadList,
                        apiName: $"todo.{env.Key}",
                        apiKey: env.Value
                    );
                    Assert.NotEmpty(response?.Data);
                    Assert.Equal(ConfigKeyForReadList.Count, response.Data.Count);
                    Assert.DoesNotContain(response.Data, o => string.IsNullOrWhiteSpace(o.Value));
                }
            }
        }
        #endregion

        public override Task<bool> IsReady()
        {
            Assert.NotEmpty(AdminApiKeyList);
            Assert.NotEmpty(ConfigKeyForSetForAllEnvMap);
            Assert.NotEmpty(ConfigServiceUrlMap);

            return base.IsReady();
        }

        protected List<string> m_CurrentProcessConfigKeys =
        [
            SysConfigKey.All_Service_HealthCheckUrl_ConfigKey,
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "sys_db",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "fruit_db",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "assets_db",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "auth_db",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "svc_ds1",
            SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "svc_crawler",
            SysConfigKey.Default_ElasticSearch_HostUrl_ConfigKey,
            SysConfigKey.Whitelist_ElasticSearch_Indices_ConfigKey,
            SysConfigKey.Default_Notification_Slack_Options_ConfigKey,
            SysConfigKey.Default_Auth_ConfigKey,
            SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey,
            SysConfigKey.Default_AWS_SQS_Urls_Measurement_ConfigKey,
            SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey,
        ];
    }
}
