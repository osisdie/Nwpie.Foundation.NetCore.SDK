using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.MiniSite.KVS.Common.Domain.Interfaces;
using Nwpie.MiniSite.KVS.Common.Utilities;

namespace Nwpie.MiniSite.KVS.Common.Domain
{
    public abstract class KvsDataAccessBase : IKvsDataAccess
    {
        public static MySql.Data.MySqlClient.MySqlConnection GetMySqlConnection() =>
            DataAccessUtils.GetMySqlConnection(KvsConfigUtils.MysqlMasterConnectionString, open: true);

        MySql.Data.MySqlClient.MySqlConnection IKvsDataAccess.GetMySqlConnection() =>
            GetMySqlConnection();

        static KvsDataAccessBase()
        {
            if (null == CacheClient)
            {
                lock (m_Lock)
                {
                    if (null == CacheClient)
                    {
                        CacheClient = ComponentMgr.Instance.GetDefaultCacheFromConfig(isHealthCheck: true);
                    }
                }
            }

        }

        public static readonly ICache CacheClient;

        private static readonly object m_Lock = new object();
    }
}
