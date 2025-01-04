using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Nwpie.Foundation.Abstractions.DataAccess.Interfaces
{
    public interface ICommandExecutor
    {
        ICommandExecutor SetParameterValue(string paramName, object paramValue);
        ICommandExecutor ToggleDynamicSection(string sectionName, bool enable);
        ICommandExecutor SetParameterValue(string paramName, DataTable dataTable, string tableTypeName);
        ICommandExecutor SetParameterValue<T>(string paramName, IEnumerable<T> list, string tableTypeName);
        ICommandExecutor SetOutputParameter(string paramName, DbType type, int size);

        Task ExecuteReaderAsync(Action<IDataReader> callback);
        Task<object> ExecuteScalarAsync();
        Task<T> ExecuteScalarAsync<T>();
        Task<List<T>> ExecuteListAsync<T>();
        Task<int> ExecuteNonQueryAsync();
        Task<T> ExecuteEntityAsync<T>() where T : class;
        Task<List<T>> ExecuteEntityListAsync<T>() where T : class;
        Task<dynamic> ExecuteDynamicAsync();
        Task<List<dynamic>> ExecuteDynamicListAsync();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
        Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>();

        int ReturnValue { get; }
        string CommandName { get; }
        Guid? ConnectionGuid { get; }
        bool IsDisposePerExecution { get; set; }
        int CommandTimeOut { get; set; }
    }
}
