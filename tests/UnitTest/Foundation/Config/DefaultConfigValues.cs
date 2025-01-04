using System;
using System.Collections.Generic;
using Amazon;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;

namespace Nwpie.xUnit.Foundation.Config
{
    public static class DefaultConfigValues
    {
        public static Dictionary<string, Dictionary<string, string>> GetPlatformValueList()
        {
            var now = DateTime.UtcNow;
            var defaultSerializer = new DefaultSerializer();
            var src = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/966385500434",
                                HomeArn = "arn:aws:sqs:us-west-2:966385500434",
                                Topic = "dev-common-notify.fifo",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/049418999959",
                                HomeArn = "arn:aws:sqs:us-west-2:049418999959",
                                Topic = "common-notify.fifo",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/049418999959",
                                HomeArn = "arn:aws:sqs:us-west-2:049418999959",
                                Topic = "common-notify.fifo",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        }
                    }
                },
                {
                    SysConfigKey.Default_Notification_Slack_Options_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new Slack_Option()
                            {
                                WebHookUrl = "https://hooks.slack.com/services/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",
                                DefaultChannel = "#foundation-notify-dev",
                                DefaultSender = $"{CommonConst.SdkPrefix}-{EnvironmentEnum.Debug.GetDisplayName()}",
                                _ts = DateTime.UtcNow
                            })
                        },
                        { EnvironmentEnum.Development.GetDisplayName(), defaultSerializer.Serialize(new Slack_Option()
                            {
                                WebHookUrl = "https://hooks.slack.com/services/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",
                                DefaultChannel = "#foundation-notify-dev",
                                DefaultSender = $"{CommonConst.SdkPrefix}-{EnvironmentEnum.Development.GetDisplayName()}",
                                _ts = DateTime.UtcNow
                            })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new Slack_Option()
                            {
                                WebHookUrl = "https://hooks.slack.com/services/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",
                                DefaultChannel = $"#foundation-notify",
                                DefaultSender = $"{CommonConst.SdkPrefix}-{EnvironmentEnum.Staging_2.GetDisplayName()}",
                                _ts = DateTime.UtcNow
                            })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new Slack_Option()
                            {
                                WebHookUrl = "https://hooks.slack.com/services/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",
                                DefaultChannel = $"#foundation-notify",
                                DefaultSender = $"{CommonConst.SdkPrefix}",
                                _ts = DateTime.UtcNow
                            })
                        }
                    }
                },
                {
                    SysConfigKey.Default_Auth_HostUrl_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), "https://api-dev.kevinw.net/auth" },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), "https://api.kevinw.net/auth" },
                        { EnvironmentEnum.Production.GetDisplayName(), "https://api.kevinw.net/auth" }
                    }
                },
                {
                    SysConfigKey.Default_Notification_HostUrl_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), "https://api-dev.kevinw.net/hub/notification" },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), "https://api.kevinw.net/hub/notification" },
                        { EnvironmentEnum.Production.GetDisplayName(), "https://api.kevinw.net/hub/notification" }
                    }
                },
                {
                    SysConfigKey.Default_Location_HostUrl_ConfigKey,
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), "https://api-dev.kevinw.net/loc" },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), "https://api.kevinw.net/loc" },
                        { EnvironmentEnum.Production.GetDisplayName(), "https://api.kevinw.net/loc" }
                    }
                },
                {
                    SysConfigKey.Default_ElasticSearch_HostUrl_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), "https://vpc-nwpie-dev-u6mumdbhoyc5racvjyv6opm7kq.us-west-2.es.amazonaws.com" },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), "https://vpc-ds1-owduq6dhfr2jrvol3nq3auyczy.us-west-2.es.amazonaws.com" },
                        { EnvironmentEnum.Production.GetDisplayName(), "https://vpc-ds1-owduq6dhfr2jrvol3nq3auyczy.us-west-2.es.amazonaws.com" }
                    }
                },
                {
                    SysConfigKey.Whitelist_ElasticSearch_Indices_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new List<string>()
                           {
                               "ds1_item",
                               "ds1_dropbox",
                               "ds1_asset",
                               "ds1_debug_dropbox",
                               "ds1_debug_asset",
                               "ds1_dev_dropbox",
                               "ds1_dev_asset"
                           })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new List<string>()
                           {
                               "ds1_item",
                               "ds1_dropbox",
                               "ds1_asset",
                               "ds1_prod_dropbox",
                               "ds1_prod_asset"
                           })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new List<string>()
                           {
                               "ds1_item",
                               "ds1_dropbox",
                               "ds1_asset",
                               "ds1_prod_dropbox",
                               "ds1_prod_asset"
                           })
                       }
                    }
                },
                {
                    SysConfigKey.Default_AWS_SQS_Urls_Measurement_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/966385500434",
                                HomeArn = "arn:aws:sqs:us-west-2:966385500434",
                                Topic = "dev-common-metric",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/049418999959",
                                HomeArn = "arn:aws:sqs:us-west-2:049418999959",
                                Topic = "common-metric",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new AwsSQS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sqs.us-west-2.amazonaws.com",
                                QueueBaseUrl = "https://sqs.us-west-2.amazonaws.com/049418999959",
                                HomeArn = "arn:aws:sqs:us-west-2:049418999959",
                                Topic = "common-metric",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        }
                    }
                },
                {
                    SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new AwsSNS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sns.us-west-2.amazonaws.com",
                                HomeArn = "arn:aws:sns:us-west-2:966385500434",
                                Topic = "dev-location-topic",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new AwsSNS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sns.us-west-2.amazonaws.com",
                                HomeArn = "arn:aws:sns:us-west-2:049418999959",
                                Topic = "location-topic",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new AwsSNS_Option()
                            {
                                Region = RegionEndpoint.USWest2.SystemName,
                                ServiceUrl = "https://sns.us-west-2.amazonaws.com",
                                HomeArn = "arn:aws:sns:us-west-2:049418999959",
                                Topic = "location-topic",
                                //AccessKey = "", // Each accessKey only apply for 1 app
                                //SecretKey = "", // Each secretKey only apply for 1 app
                                _ts = now
                            })
                        }
                    }
                },
                {
                    SysConfigKey.All_Service_HealthCheckUrl_ConfigKey,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                        { EnvironmentEnum.Debug.GetDisplayName(), defaultSerializer.Serialize(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "foundation", "https://api-dev.kevinw.net/foundation/healthcheck/echo" },
                                { "config", "https://api-dev.kevinw.net/config/healthcheck/echo" },
                                { "loc", "https://api-dev.kevinw.net/loc/health" },
                                { "hub", "https://api-dev.kevinw.net/hub/health" },
                                { "auth", "https://api-dev.kevinw.net/auth/health" },
                                { "es", "https://api-dev.kevinw.net/es/health" },
                                { "storage", "https://api-dev.kevinw.net/storage/health" },
                                { "acct", "https://api-dev.kevinw.net/acct/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "ds1", "https://api-dev.kevinw.net/ds1/healthcheck/echo" },
                                { "ds1-directly", "https://api-dev.kevinw.net/ds1-directly/health" },
                                { "todo", "https://api-dev.kevinw.net/todo/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "fruit", "https://api-dev.kevinw.net/de/health" },
                                { "fruit-store", "https://api-dev.kevinw.net/fruit-store/health" }
                            })
                        },
                        { EnvironmentEnum.Staging_2.GetDisplayName(), defaultSerializer.Serialize(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "foundation", "https://api.kevinw.net/foundation/healthcheck/echo" },
                                { "config", "https://api.kevinw.net/config/healthcheck/echo" },
                                { "loc", "https://api.kevinw.net/loc/health" },
                                { "hub", "https://api.kevinw.net/hub/health" },
                                { "auth", "https://api.kevinw.net/auth/health" },
                                { "es", "https://api.kevinw.net/es/health" },
                                { "storage", "https://api.kevinw.net/storage/health" },
                                { "acct", "https://api.kevinw.net/acct/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "ds1", "https://api.kevinw.net/ds1/healthcheck/echo" },
                                { "ds1-directly", "https://api.kevinw.net/ds1-directly/health" },
                                { "todo", "https://api.kevinw.net/todo/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "fruit", "https://api-preprod.kevinw.net/de/health" },
                                { "fruit-store", "https://api.kevinw.net/fruit-store/health" }
                            })
                        },
                        { EnvironmentEnum.Production.GetDisplayName(), defaultSerializer.Serialize(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "foundation", "https://api.kevinw.net/foundation/healthcheck/echo" },
                                { "config", "https://api.kevinw.net/config/healthcheck/echo" },
                                { "loc", "https://api.kevinw.net/loc/health" },
                                { "hub", "https://api.kevinw.net/hub/health" },
                                { "auth", "https://api.kevinw.net/auth/health" },
                                { "es", "https://api.kevinw.net/es/health" },
                                { "storage", "https://api.kevinw.net/storage/health" },
                                { "acct", "https://api.kevinw.net/acct/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "ds1", "https://api.kevinw.net/ds1/healthcheck/echo" },
                                { "ds1-directly", "https://api.kevinw.net/ds1-directly/health" },
                                { "todo", "https://api.kevinw.net/todo/healthcheck/HlckEchoRequest?format=json&data={requestString:123}" },
                                { "fruit", "https://api.kevinw.net/de/health" },
                                { "fruit-store", "https://api.kevinw.net/fruit-store/health" }
                            })
                        }
                    }
                }
            };

            return src;
        }
    }
}
