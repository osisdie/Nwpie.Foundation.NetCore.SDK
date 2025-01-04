using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class BaseCommand : CObject, ICommand
    {
        static BaseCommand()
        {
            m_Serializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
        }

        public BaseCommand() { }

        #region CommandExecutors
        public async Task ExecuteReaderAsync(Action<IDataReader> callback)
        {
            var connection = GetOrCreateConnection();
            using (IsDisposeConnectionPerExecution ? connection : null)
            {
                connection.Open();
                using (var dataReader = await connection.ExecuteReaderAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                ))
                {
                    callback(dataReader);
                }
            }
        }

        public async Task<object> ExecuteScalarAsync() =>
            await ExecutionEntry(async conn =>
            {
                return await conn.ExecuteScalarAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );
            });

        public async Task<T> ExecuteScalarAsync<T>() =>
            await ExecutionEntry(async conn =>
            {
                return await conn.ExecuteScalarAsync<T>(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );
            });

        public async Task<IEnumerable<T>> ExecuteListAsync<T>() =>
            await ExecutionEntry(async conn =>
            {
                return await conn.QueryAsync<T>(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );
            });

        public async Task<int> ExecuteNonQueryAsync() =>
            await ExecutionEntry(async conn =>
            {
                var result = ConfigConst.DefaultDBExecutionResult;
                var obj = await conn.ExecuteScalarAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );

                int.TryParse(obj?.ToString(), out result);

                return result;
            });

        public async Task<T> ExecuteEntityAsync<T>() where T : class
        {
            return await ExecutionEntry(async conn =>
            {
                var items = await conn.QueryAsync<T>(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );

                return items?.FirstOrDefault();
            });
        }

        public async Task<IEnumerable<T>> ExecuteEntityListAsync<T>()
            where T : class
        {
            return await ExecutionEntry(async conn =>
            {
                return await conn.QueryAsync<T>(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );
            });
        }

        public async Task<dynamic> ExecuteDynamicAsync() =>
            await ExecutionEntry(async connection =>
            {
                dynamic result = new ExpandoObject();
                var items = await connection.QueryAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );

                return items?.Single();
            });

        public async Task<IEnumerable<dynamic>> ExecuteDynamicListAsync() =>
            await ExecutionEntry(async connection =>
            {
                dynamic result = new ExpandoObject();
                result = await connection.QueryAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    //buffered: true,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );

                return result;
            });

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            (int readCount)
        {
            var lists = new IEnumerable[readCount];
            var gr = await ExecutionEntry(async connection =>
            {
                return await connection.QueryMultipleAsync(sql: CommandText,
                    param: Parameters,
                    transaction: null,
                    commandTimeout: CommandTimeout,
                    commandType: CommandType
                );
            });

            if (null != gr)
            {
                if (readCount >= 1 && false == gr.IsConsumed)
                {
                    lists[0] = gr.Read<T1>();
                    if (readCount >= 2 && false == gr.IsConsumed)
                    {
                        lists[1] = gr.Read<T2>();
                        if (readCount >= 3 && false == gr.IsConsumed)
                        {
                            lists[2] = gr.Read<T3>();
                            if (readCount >= 4 && false == gr.IsConsumed)
                            {
                                lists[3] = gr.Read<T4>();
                                if (readCount >= 5 && false == gr.IsConsumed)
                                {
                                    lists[4] = gr.Read<T5>();
                                    if (readCount >= 6 && false == gr.IsConsumed)
                                    {
                                        lists[5] = gr.Read<T6>();
                                        if (readCount >= 7 && false == gr.IsConsumed)
                                        {
                                            lists[6] = gr.Read<T7>();
                                            if (readCount >= 8 && false == gr.IsConsumed)
                                            {
                                                lists[7] = gr.Read<T8>();
                                                if (readCount >= 9 && false == gr.IsConsumed)
                                                {
                                                    lists[8] = gr.Read<T9>();
                                                    if (readCount >= 10 && false == gr.IsConsumed)
                                                    {
                                                        lists[9] = gr.Read<T10>();
                                                        if (readCount >= 11 && false == gr.IsConsumed)
                                                        {
                                                            lists[10] = gr.Read<T11>();
                                                            if (readCount >= 12 && false == gr.IsConsumed)
                                                            {
                                                                lists[11] = gr.Read<T12>();
                                                                if (readCount >= 13 && false == gr.IsConsumed)
                                                                {
                                                                    lists[12] = gr.Read<T13>();
                                                                    if (readCount >= 14 && false == gr.IsConsumed)
                                                                    {
                                                                        lists[13] = gr.Read<T14>();
                                                                        if (readCount >= 15 && false == gr.IsConsumed)
                                                                        {
                                                                            lists[14] = gr.Read<T15>();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return lists;
        }
        #endregion

        #region Connection
        public IDbConnection GetOrCreateConnection()
        {
            if (null != CurrentConnection) // && CurrentConnection.State == ConnectionState.Closed)
            {
                return CurrentConnection;
            }

            CurrentConnection = CreateConnection();
            ConnectionGuid = Guid.NewGuid();

            return CurrentConnection;
        }

        protected IDbConnection CreateConnection() => Provider switch
        {
            DataSourceEnum.MySQL => new MySqlConnection(ConnectionString),
            DataSourceEnum.SqlServer => new SqlConnection(ConnectionString),
            _ => throw new NotSupportedException(Provider.ToString())
        };

        protected async Task<T> ExecutionEntry<T>(Func<IDbConnection, Task<T>> getData, [CallerMemberName] string caller = "")
        {
            var startTick = DateTime.UtcNow;
            Exception lastEx = null;
            var isTimeout = false;
            try
            {
                var conn = GetOrCreateConnection();
                using (IsDisposeConnectionPerExecution ? conn : null)
                {
                    if (ConnectionState.Closed == conn.State)
                    {
                        conn.Open();
                    }

                    return await getData(conn);
                }
            }
            catch (TimeoutException ex)
            {
                lastEx = ex;
                isTimeout = true;
            }
            catch (Exception ex)
            {
                lastEx = ex;
                isTimeout = (DateTime.UtcNow - startTick)
                    .TotalSeconds >= CommandTimeout;
            }
            finally
            {
                if (null != lastEx)
                {
                    Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { SysLoggerKey.Type, LoggingTypeEnum.DALException.GetDisplayName() },
                        { SysLoggerKey.IsTimeout, isTimeout },
                        { SysLoggerKey.Provider, Provider.ToString() },
                        { SysLoggerKey.Database, CurrentConnection?.Database },
                        { SysLoggerKey.ConnectionGuid, ConnectionGuid },
                        { SysLoggerKey.ConnectionTimeout, CurrentConnection?.ConnectionTimeout },
                        { SysLoggerKey.CommandTimeout, CommandTimeout },
                        { SysLoggerKey.CommandText, CommandText },
                        { SysLoggerKey.CommandType, CommandType.ToString() },
                        { SysLoggerKey.Paramters, Parameters },
                        { SysLoggerKey.Caller, caller },
                        { SysLoggerKey.Exception, lastEx?.ToString() },
                    }.AddTraceData(startTick)));
                }

                if (null != lastEx)
                {
                    throw lastEx;
                }
            }

            return default(T);
        }
        #endregion

        public static readonly ISerializer m_Serializer;

        public int CommandTimeout { get; set; } = ConfigConst.DefaultDALTimeout;
        public string CommandText { get; set; }
        public string ConnectionString { get; set; }
        public bool IsDisposeConnectionPerExecution { get; set; } = true;
        public DataSourceEnum Provider { get; set; }
        public CommandType CommandType { get; set; }
        public DynamicParameters Parameters { get; set; }
        public IDbConnection CurrentConnection { get; set; }
        public Guid? ConnectionGuid { get; private set; }
    }
}
