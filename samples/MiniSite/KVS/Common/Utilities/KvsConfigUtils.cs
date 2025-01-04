using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Entities;

namespace Nwpie.MiniSite.KVS.Common.Utilities
{
    public static class KvsConfigUtils
    {
        public static string DbConnectionStringKey
        {
            get => SysConfigKey.PrefixKey_AWS_Mysql_ConnectionString_ConfigKey + KvsConst.DefaultDatabaseName;
        }

        private static string m_MysqlMasterConnectionString;
        public static string MysqlMasterConnectionString
        {
            get
            {
                if (null != m_MysqlMasterConnectionString)
                {
                    return m_MysqlMasterConnectionString;
                }

                m_MysqlMasterConnectionString = DbConnectionStringKey
                    .ConfigServerRawValue();
                if (null == m_MysqlMasterConnectionString)
                {
                    m_MysqlMasterConnectionString = ConfigurationManager
                        .ConnectionStrings[DbConnectionStringKey]
                        ?.ConnectionString
                        ?? throw new MissingFieldException(nameof(DbConnectionStringKey));
                }

                return m_MysqlMasterConnectionString;
            }
        }

        private static string m_PlatformAppId;
        public static string PlatformAppId
        {
            get
            {
                if (null != m_PlatformAppId)
                {
                    return m_PlatformAppId;
                }

                var sysName = $"sdk.sys_name".ConfigServerRawValue();
                var app = ApplicationDomainHelper
                    .GetApplicationByNameAsync(sysName).ConfigureAwait(false).GetAwaiter().GetResult();
                m_PlatformAppId = app?.app_id
                    ?? throw new MissingFieldException(nameof(m_PlatformAppId));

                return m_PlatformAppId;
            }
        }
        public static string PlatformBaseApiKey => PlatformAppId;

        private static List<API_KEY_Entity> m_PlatformApiKeyList;
        public static List<API_KEY_Entity> PlatformApiKeyList
        {
            get
            {
                if (null != m_PlatformApiKeyList)
                {
                    return m_PlatformApiKeyList;
                }

                m_PlatformApiKeyList = ApiKeyDomainHelper.ListApiKeyAsync(PlatformAppId).ConfigureAwait(false).GetAwaiter().GetResult()?.ToList();
                if (true != (m_PlatformApiKeyList?.Count() > 0))
                {
                    throw new MissingFieldException(nameof(m_PlatformApiKeyList));
                }

                return m_PlatformApiKeyList;
            }
        }

        private static Auth_Option m_AuthOption;
        public static Auth_Option AuthOption
        {
            get
            {
                if (null != m_AuthOption)
                {
                    return m_AuthOption;
                }

                m_AuthOption = SysConfigKey
                    .Default_Auth_ConfigKey
                    .ConfigServerValue<Auth_Option>()
                    ?? throw new MissingFieldException(nameof(m_AuthOption));

                return m_AuthOption;
            }
        }
    }
}
