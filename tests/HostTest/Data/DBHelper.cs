using System.Collections.Generic;
using System.Linq;
using Nwpie.HostTest.Models;
using Dapper;

namespace Nwpie.HostTest.Data
{
    public static class DBHelper
    {
        static DBHelper()
        {
            var conns = Startup.Configuration.GetSection("connectionStrings")
                .GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);

            m_Factory = new DbConnectionFactory(conns);
        }

        public static IEnumerable<T> ExecCustomerOrderDetailView<T>(int custId, string connectionStringName = null)
        {
            using (var conn = m_Factory.CreateDbConnection(connectionStringName ?? DefaultConnectionName))
            {
                conn.Open();
                return conn.Query<T>(@"
SELECT *
FROM uv_CustomerOrderDetails
where CustId = @CustId
", new { CustId = custId });
            }
        }

        public static CustomerInfo GetCustomerOrderDetail(int custId, string connectionStringName = null)
        {
            var entities = DBHelper.ExecCustomerOrderDetailView<CustomerOrderDetail_Entity>(custId, connectionStringName);
            return entities.ToFirstCustomerInfo();
        }

        public static CustomerInfo CachedCustomerInfo(int custId, string connectionStringName = null)
        {
            using (var conn = m_Factory.CreateDbConnection(connectionStringName ?? DefaultConnectionName))
            {
                conn.Open();
                return conn.QueryFirstOrDefault<CustomerInfo>(@"
SELECT *
FROM Customer
where CustId = @CustId
", new { CustId = custId });
            }
        }

        public static OrderInfo CachedOrderInfo(int orderId, string connectionStringName = null)
        {
            using (var conn = m_Factory.CreateDbConnection(connectionStringName ?? DefaultConnectionName))
            {
                conn.Open();
                return conn.QueryFirstOrDefault<OrderInfo>(@"
SELECT *
FROM [Order]
where OrderId = @OrderId
", new { OrderId = orderId });
            }
        }

        public static IEnumerable<OrdDetails> CachedOrderDetails(int orderId, string connectionStringName = null)
        {
            using (var conn = m_Factory.CreateDbConnection(connectionStringName ?? DefaultConnectionName))
            {
                conn.Open();
                return conn.Query<OrdDetails>(@"
SELECT *
FROM OrdDetail
where OrderId = @OrderId
", new { OrderId = orderId });
            }
        }

        public static string DefaultConnectionName = "DefaultConnection";

        private static readonly DbConnectionFactory m_Factory;
    }
}
