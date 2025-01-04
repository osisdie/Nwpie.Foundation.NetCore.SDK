using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Nwpie.HostTest.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection(string connectionName);
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDictionary<string, string> _connectionDict;

        public DbConnectionFactory(IDictionary<string, string> connectionDict)
        {
            _connectionDict = connectionDict;
        }

        public IDbConnection CreateDbConnection(string connectionName)
        {
            if (_connectionDict.TryGetValue(connectionName, out var connectionString))
            {
                return new SqlConnection(connectionString);
            }

            throw new ArgumentNullException();
        }
    }
}
