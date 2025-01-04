using System;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.MiniSite.Storage.Common.Utilities
{
    public static class CustomConfigUtils
    {
        static string m_ServiceEmails;
        public static string ServiceEmails
        {
            get
            {
                if (null != m_ServiceEmails)
                {
                    return m_ServiceEmails;
                }

                m_ServiceEmails = "app:serviceMail"
                    .ConfigServerRawValue()
                    ?? throw new MissingFieldException(nameof(ServiceEmails));

                return m_ServiceEmails;
            }
        }


        static string m_DefaultBucketName;
        public static string DefaultBucketName
        {
            get
            {
                if (null != m_DefaultBucketName)
                {
                    return m_DefaultBucketName;
                }

                m_DefaultBucketName = "app:services:Storage:defaultBucketName"
                    .ConfigServerRawValue()
                    ?? throw new MissingFieldException(nameof(DefaultBucketName));

                return m_DefaultBucketName;
            }
        }

        static Auth_Option m_AuthOption;
        public static Auth_Option AuthOption
        {
            get
            {
                if (null != m_AuthOption)
                {
                    return m_AuthOption;
                }

                m_AuthOption = ServiceContext.Configuration.GetValue<Auth_Option>(
                    SysConfigKey.Default_Auth_ConfigKey,
                    ComponentMgr.Instance.TryResolve<ISerializer>()
                ) ?? throw new MissingFieldException(nameof(Auth_Option));

                return m_AuthOption;
            }
        }

        static AwsSQS_Option m_AwsSqsOption;
        public static AwsSQS_Option AwsSqsOption
        {
            get
            {
                if (null != m_AwsSqsOption)
                {
                    return m_AwsSqsOption;
                }

                m_AwsSqsOption = SysConfigKey
                    .Default_AWS_SQS_Urls_Notification_ConfigKey
                    .ConfigServerValue<AwsSQS_Option>()
                    ?? throw new MissingFieldException(nameof(AwsSQS_Option));

                return m_AwsSqsOption;
            }
        }
    }
}
