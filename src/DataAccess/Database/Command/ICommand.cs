using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.Models;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface ICommand : ICObject
    {
        #region Connection
        IDbConnection GetOrCreateConnection();
        #endregion

        #region CommandExecutors
        Task ExecuteReaderAsync(Action<IDataReader> callback);
        Task<object> ExecuteScalarAsync();
        Task<T> ExecuteScalarAsync<T>();
        Task<IEnumerable<T>> ExecuteListAsync<T>();
        Task<int> ExecuteNonQueryAsync();

        Task<T> ExecuteEntityAsync<T>()
            where T : class;
        Task<IEnumerable<T>> ExecuteEntityListAsync<T>()
            where T : class;

        Task<dynamic> ExecuteDynamicAsync();
        Task<IEnumerable<dynamic>> ExecuteDynamicListAsync();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(int readCount);
        #endregion

        bool IsDisposeConnectionPerExecution { get; set; }
        string ConnectionString { get; set; }
        string CommandText { get; set; }
        int CommandTimeout { get; set; }
        CommandType CommandType { get; set; }
        DataSourceEnum Provider { get; set; }
        DynamicParameters Parameters { get; set; }
        IDbConnection CurrentConnection { get; set; }
        Guid? ConnectionGuid { get; }
    }
}
