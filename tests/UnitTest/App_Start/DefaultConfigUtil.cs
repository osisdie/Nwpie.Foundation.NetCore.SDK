using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.xUnit.Foundation.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceStack;

namespace Nwpie.xUnit.App_Start
{
    public static class DefaultConfigUtil
    {
        public static void InitialConfigure()
        {
            IConfigurationBuilder config = new ConfigurationBuilder();
            ServiceContext.Configuration = config.SdkBuild().Build();

            DefaultCacheUtil.OverwriteRedisCacheConfig();
            DefaultAuthUtil.OverwriteAuthAESConfig();

            IsValid();

            TestBase.ConfigKeyForReadList = GetReadList();
            TestBase.ConfigKeyForSetForAllEnvMap = DefaultConfigValues.GetPlatformValueList();
            TestBase.ConfigServiceUrlMap = GetKeyValueListInSection("sdk.config.host_url", new Regex("^[a-z]+$"))?.ToDictionary(x => x.Key, x => x.Value);
            TestBase.LocationServiceUrlMap = GetKeyValueListInSection("sdk.location.host_url", new Regex("^[a-z]+$"))?.ToDictionary(x => x.Key, x => x.Value);
            TestBase.AuthServiceUrlMap = GetKeyValueListInSection("sdk.auth.host_url", new Regex("^[a-z]+$"))?.ToDictionary(x => x.Key, x => x.Value);
            TestBase.AdminApiKeyList = GetKeyValueListInSection("sdk.api.list", new Regex("^sdk.admin.[a-z]+$"))?.ToList();
            TestBase.NtfyApiKeyList = GetKeyValueListInSection("sdk.api.list", new Regex("^ntfy.[a-z]+$"))?.ToList();
            TestBase.TodoApiKeyList = GetKeyValueListInSection("sdk.api.list", new Regex("^todo.[a-z]+$"))?.ToList();
        }

        public static IConfigurationBuilder SdkBuild(this IConfigurationBuilder config)
        {
            JsonConvert.DefaultSettings = () => new DefaultSerializer().Settings;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // 30 Day Free Trial
            //Licensing.RegisterLicense("*****");

            config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddProvider(ServiceContext.Config);

            return config;
        }

        public static bool IsValid()
        {
            var notifyMessageQueueOption = SysConfigKey
                .Default_AWS_SQS_Urls_Notification_ConfigKey
                .ConfigServerValue<AwsSQS_Option>();

            var measurementMessageQueueOption = SysConfigKey
                .Default_AWS_SQS_Urls_Measurement_ConfigKey
                .ConfigServerValue<AwsSQS_Option>();

            var s3Option = SysConfigKey
                .Default_AWS_S3_Credential_ConfigKey
                .ConfigServerValue<AwsS3_Option>();

            var redis = SysConfigKey
                .Default_AWS_Redis_ConnectionString_ConfigKey
                .ConfigServerRawValue();

            var qcmysql = (SysConfigKey
                .PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db")
                .ConfigServerRawValue();

            var qcs3region = (SysConfigKey
                .PrefixKey_AWS_S3_Credential_ConfigKey + "todo_db")
                .ConfigServerRawValue();

            return true;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetKeyValueListInSection(string section, Regex pattern)
        {
            var list = ServiceContext.Configuration.GetKeyValueListInSection(section, pattern);
            foreach (var item in list)
            {
                yield return KeyValuePair.Create(Utility.GetSDKEnvNameByApiName(item.Key), item.Value);
            }
        }

        public static List<ConfigItem> GetReadList()
        {
            return new List<ConfigItem>() {
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_HostUrl_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_AWS_SQS_Urls_Measurement_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_AWS_S3_Credential_ConfigKey},
                new ConfigItem(){ ConfigKey = SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "todo_db" },
                new ConfigItem(){ ConfigKey = SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + "auth_db" },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_AWS_Redis_ConnectionString_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Auth_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Auth_HostUrl_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_Smtp_Options_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_LINE_Options_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_Slack_Options_ConfigKey },
            };
        }
    }
}
