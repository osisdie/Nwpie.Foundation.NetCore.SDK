using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.DataAccess.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.DataAccess.Database.Measurement;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class CommandExecutor : CObject, ICommandExecutor
    {
        public CommandExecutor(string cmdName)
        {
            m_CommandBuilder = new FlexibleCommandBuilder(cmdName)
                ?? throw new ArgumentNullException(nameof(cmdName));
        }

        public CommandExecutor(string cmdText, string connStr, int cmdTimeout = 60, CommandType cmdType = CommandType.StoredProcedure, DataSourceEnum provider = DataSourceEnum.UnSet)
        {
            if (DataSourceEnum.UnSet == provider)
                throw new ArgumentException(nameof(provider));

            m_CommandBuilder = new FlexibleCommandBuilder(cmdText, connStr, cmdTimeout, cmdType, provider);
        }

        protected ICommand Build()
        {
            if (false == IsDisposePerExecution && null != CurrentCommand)
            {
                return CurrentCommand;
            }

            if (CommandTimeOut > 0)
            {
                m_CommandBuilder.CommandConfigInfo.CommandTimeout = CommandTimeOut;
            }

            if (m_ConnectionString.HasValue())
            {
                m_CommandBuilder.CommandConfigInfo.ConnectionString = m_ConnectionString;
            }

            var command = m_CommandBuilder.Build();
            command.IsDisposeConnectionPerExecution = IsDisposePerExecution;
            return command;
        }

        public async Task ExecuteReaderAsync(Action<IDataReader> callback)
        {
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                await CurrentCommand.ExecuteReaderAsync((dataReader) =>
                {
                    callback(dataReader);
                });
            });
        }

        public async Task<object> ExecuteScalarAsync()
        {
            object result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand.ExecuteScalarAsync();
            });

            return result;
        }

        public async Task<T> ExecuteScalarAsync<T>()
        {
            var result = default(T);
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand.ExecuteScalarAsync<T>();
            });

            return result;
        }

        public async Task<List<T>> ExecuteListAsync<T>()
        {
            List<T> result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                var items = await CurrentCommand.ExecuteListAsync<T>();
                if (items?.Count() > 0)
                {
                    result = items.ToList();
                }
            });

            return result;
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            var result = ConfigConst.DefaultDBExecutionResult;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand.ExecuteNonQueryAsync();
            });

            return result;
        }

        public async Task<T> ExecuteEntityAsync<T>()
            where T : class
        {
            var result = default(T);
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand.ExecuteEntityAsync<T>();
            });

            return result;
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>()
            where T : class
        {
            List<T> result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                var items = await CurrentCommand.ExecuteEntityListAsync<T>();
                if (items?.Count() > 0)
                {
                    result = items.ToList();
                }
            });

            return result;
        }

        public async Task<dynamic> ExecuteDynamicAsync()
        {
            dynamic result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand.ExecuteDynamicAsync();
            });

            return result;
        }

        public async Task<List<dynamic>> ExecuteDynamicListAsync()
        {
            List<dynamic> result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                var items = await CurrentCommand.ExecuteDynamicListAsync();
                if (items?.Count() > 0)
                {
                    result = items.ToList();
                }
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, object, object, object, object, object, object, object, object, object, object, object, object, object>(2)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, object, object, object, object, object, object, object, object, object, object, object, object>(3)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, object, object, object, object, object, object, object, object, object, object, object>(4)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, object, object, object, object, object, object, object, object, object, object>(5)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, object, object, object, object, object, object, object, object, object>(6)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, object, object, object, object, object, object, object, object>(7)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, object, object, object, object, object, object, object>(8)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, object, object, object, object, object, object>(9)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object, object, object, object, object>(10)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object, object, object, object>(11)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object, object, object>(12)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object, object>(13)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>(14)
                    ;
            });

            return result;
        }

        public async Task<IEnumerable[]> ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
        {
            IEnumerable[] result = null;
            CurrentCommand = Build();
            await this.TryExecuteAsync(async () =>
            {
                result = await CurrentCommand
                    .ExecuteMultipleQueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(15)
                    ;
            });

            return result;
        }

        public ICommandExecutor SetParameterValue(string paramName, object paramValue)
        {
            m_CommandBuilder.SetParameterValue(paramName, paramValue);
            return this;
        }

        public ICommandExecutor ToggleDynamicSection(string sectionName, bool enable)
        {
            m_CommandBuilder.ToggleDynamicSection(sectionName, enable);
            return this;
        }

        public ICommandExecutor SetParameterValue(string paramName, DataTable dataTable, string tableTypeName)
        {
            m_CommandBuilder.SetParameterValue(paramName, dataTable, tableTypeName);
            return this;
        }

        public ICommandExecutor SetParameterValue<T>(string paramName, IEnumerable<T> list, string tableTypeName)
        {
            m_CommandBuilder.SetParameterValue(paramName, list, tableTypeName);
            return this;
        }

        public ICommandExecutor SetOutputParameter(string paramName, DbType type, int size)
        {
            m_CommandBuilder.SetOutputParameter(paramName, type, size);
            return this;
        }

        public int ReturnValue
        {
            get
            {
                try
                {
                    return Parameters?.Get<int>("@returnValue") ?? ConfigConst.DefaultDBExecutionResult;
                }
                catch
                {
                    return ConfigConst.DefaultDBExecutionResult;
                }
            }
        }

        public int CommandTimeOut { get; set; }
        public ICommand CurrentCommand { get; private set; }
        public Guid? ConnectionGuid
        {
            get => CurrentCommand?.ConnectionGuid;
        }

        public bool IsDisposePerExecution { get; set; } = true;
        public DynamicParameters Parameters
        {
            get => CurrentCommand?.Parameters;
        }

        public string CommandName
        {
            get => m_CommandBuilder.CommandConfigInfo?.CommandName;
        }

        protected string m_ConnectionString { get; set; }

        private readonly ICommandBuilder m_CommandBuilder;
    }
}
