namespace Nwpie.Foundation.Abstractions.Statics
{
    public static class SysConfigKey
    {
        public const string Default_ElasticSearch_HostUrl_ConfigKey = "foundation.service.elasticsearch.host_url.default";
        public const string Whitelist_ElasticSearch_Indices_ConfigKey = "foundation.service.elasticsearch.indices.all";
        public const string Default_Notification_HostUrl_ConfigKey = "foundation.service.notification.host_url.default";
        public const string Default_Location_HostUrl_ConfigKey = "foundation.service.location.host_url.default";
        public const string Default_EmailTemplate_ConfigKey = "foundation.resource.email.template.default";

        public const string PrefixKey_Service_BaseUrl_ConfigKey = "foundation.service.location.base_url.";
        public const string PrefixKey_AWS_ES_ConnectionString_ConfigKey = "foundation.aws.es.connectionstring.";
        public const string PrefixKey_AWS_Mysql_ConnectionString_ConfigKey = "foundation.aws.mysql.connectionstring.";
        public const string Default_AWS_ES_ConnectionString_ConfigKey = "foundation.aws.es.connectionstring.default";
        public const string All_Service_BaseUrl_ConfigKey = "foundation.service.location.base_url.all";
        public const string All_Service_HealthCheckUrl_ConfigKey = "foundation.service.health.get_url.all";

        public static readonly string PrefixKey_Auth_ConfigKey = $"foundation.{CommonConst.SdkPrefix.ToLower()}.auth.";
        public static readonly string Default_Auth_ConfigKey = $"foundation.{CommonConst.SdkPrefix.ToLower()}.auth.default";
        public static readonly string Default_Auth_HostUrl_ConfigKey = $"foundation.{CommonConst.SdkPrefix.ToLower()}.auth.host_url.default";

        public const string PrefixKey_MessageQueue_ConfigKey = "foundation.mq.";
        public const string Default_RabbitMQ_ConfigKey = "foundation.mq.rabbit.default";

        public const string PrefixKey_AWS_S3_Credential_ConfigKey = "foundation.aws.s3.credential.";
        public const string Default_AWS_S3_Credential_ConfigKey = "foundation.aws.s3.credential.default";
        public const int DefaultAssetUrlExpireMinutes = 1440;

        public const string PrefixKey_AWS_Redis_ConnectionString_ConfigKey = "foundation.aws.redis.connectionstring.";
        public const string Default_AWS_Redis_ConnectionString_ConfigKey = "foundation.aws.redis.connectionstring.db0";
        public const string PrefixKey_AWS_Redis_Options_ConfigKey = "foundation.aws.redis.";
        public const string Default_AWS_Redis_Options_ConfigKey = "foundation.aws.redis.default";

        public const string PrefixKey_AWS_SQS_Urls_ConfigKey = "foundation.aws.sqs.urls.";
        public const string Default_AWS_SQS_Urls_Notification_ConfigKey = "foundation.aws.sqs.urls.notification.default";
        public const string Default_AWS_SQS_Urls_Measurement_ConfigKey = "foundation.aws.sqs.urls.measurement.default";

        public const string PrefixKey_AWS_SNS_Urls_ConfigKey = "foundation.aws.sns.urls.";
        public const string Default_AWS_SNS_Urls_Configserver_ConfigKey = "foundation.aws.sns.urls.configserver.default";
        public const string Default_AWS_SNS_Urls_Location_ConfigKey = "foundation.aws.sns.urls.location.default";

        public const string PrefixKey_Notification_LINE_Options_ConfigKey = "foundation.notification.line.";
        public const string Default_Notification_LINE_Options_ConfigKey = "foundation.notification.line.default";

        public const string PrefixKey_Notification_Slack_Options_ConfigKey = "foundation.notification.slack.";
        public const string Default_Notification_Slack_Options_ConfigKey = "foundation.notification.slack.default";

        public const string PrefixKey_Notification_Smtp_Options_ConfigKey = "foundation.notification.smtp.";
        public const string Default_Notification_Smtp_Options_ConfigKey = "foundation.notification.smtp.default";
    }
}
