using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.DataAccess.Database.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class DataCommand : CObject, IDataCommand
    {
        #region Config
        public IDataCommand SetDbProviderFactory(DbProviderFactory Dbfactory)
        {
            m_Factory = Dbfactory;
            return this;
        }

        public IDataCommand SetConectString(string connectString)
        {
            m_ConnectionString = connectString;
            return this;
        }

        public IDataCommand SetTimeOut(int timeout)
        {
            m_Timeout = timeout;
            return this;
        }

        public IDataCommand SetCommand(string cmd)
        {
            m_Cmd = cmd;
            return this;
        }

        public IDataCommand SetCommandText(string commandTest)
        {
            m_CommandTest = commandTest;
            return this;
        }
        public IDataCommand SetCommandType(CommandType type)
        {
            m_Type = type;
            return this;
        }

        public IDataCommand SetTransaction(DbTransaction trans)
        {
            m_Trans = trans;
            return this;
        }

        public IDataCommand AddParameter(DbParameter parameter)
        {
            m_Parameters.Add(parameter);
            return this;
        }

        public IDataCommand AddParameter<TParameter>(string name, TParameter value)
        {
            object innerValue = value;

            // handle value type by using actual value
            var valueType = null != value
                ? value.GetType()
                : typeof(TParameter);

            var parameter = CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = innerValue
                ?? DBNull.Value;
            parameter.DbType = valueType
                .GetUnderlyingType()
                .ToDbType();
            parameter.Direction = ParameterDirection.Input;

            return AddParameter(parameter);
        }

        /// <summary>
        /// Adds a new out parameter with the specified <paramref name="name" /> and <paramref name="callback" />.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter value.</typeparam>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="callback">The callback used to get the out value.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to the data command.
        /// </returns>
        public IDataCommand AddParameterOut<TParameter>(string name, Action<TParameter> callback)
        {
            var parameter = CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = typeof(TParameter)
                .GetUnderlyingType()
                .ToDbType();
            parameter.Direction = ParameterDirection.Output;
            // output parameters must have a size, default to MAX
            parameter.Size = -1;

            RegisterCallback(parameter, callback);
            return AddParameter(parameter);
        }

        /// <summary>
        /// Adds a new out parameter with the specified <paramref name="name" />, <paramref name="value" /> and <paramref name="callback" />.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter value.</typeparam>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to be added.</param>
        /// <param name="callback">The callback used to get the out value.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to the data command.
        /// </returns>
        public IDataCommand AddParameterOut<TParameter>(string name, TParameter value, Action<TParameter> callback)
        {
            object innerValue = value;

            var parameter = CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = innerValue
                ?? DBNull.Value;
            parameter.DbType = typeof(TParameter)
                .GetUnderlyingType()
                .ToDbType();
            parameter.Direction = ParameterDirection.InputOutput;

            RegisterCallback(parameter, callback);
            return AddParameter(parameter);
        }

        /// <summary>
        /// Adds a new return parameter with the specified <paramref name="callback" />.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter value.</typeparam>
        /// <param name="callback">The callback used to get the return value.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to the data command.
        /// </returns>
        public IDataCommand ReturnValue<TParameter>(Action<TParameter> callback)
        {
            var parameter = CreateParameter();
            parameter.ParameterName = SPResultParameterName;
            parameter.DbType = typeof(TParameter)
                .GetUnderlyingType()
                .ToDbType();
            parameter.Direction = ParameterDirection.ReturnValue;

            RegisterCallback(parameter, callback);
            return AddParameter(parameter);
        }

        #endregion

        public virtual DbParameter CreateParameter() =>
            m_Factory.CreateParameter();

        protected void RegisterCallback<TParameter>(DbParameter parameter, Action<TParameter> callback)
        {
            m_Callbacks.Enqueue(new DataCallback
            {
                Callback = callback,
                Type = typeof(TParameter),
                Parameter = parameter
            });
        }

        protected void TriggerCallbacks()
        {
            while (m_Callbacks.Count() > 0)
            {
                m_Callbacks.Dequeue()?.Invoke();
            }
        }

        class DataCallback
        {
            /// <summary>
            /// Invokes the <see cref="Callback"/> with the <see cref="Parameter"/> value.
            /// </summary>
            public void Invoke()
            {
                var value = Parameter.Value;
                if (value == DBNull.Value)
                {
                    value = default(Type);
                }

                Callback.DynamicInvoke(value);
            }

            /// <summary>
            /// Gets or sets the type of the call back value.
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// Gets or sets the callback <see langword="delegate"/>.
            /// </summary>
            public Delegate Callback { get; set; }
            /// <summary>
            /// Gets or sets the parameter associated with the callback.
            /// </summary>
            public DbParameter Parameter { get; set; }
        }

        #region Execute
        public int ExecuteNonQuery()
        {
            var cmd = CreateCommand();
            return Execture(cmd, cmd.ExecuteNonQuery);
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            var cmd = CreateCommand();
            return Execture(cmd, cmd.ExecuteNonQueryAsync);
        }

        public DbDataReader ExecuteReader()
        {
            var cmd = CreateCommand();
            if (null == m_Trans)
            {
                var conn = CreateConnection();
                try
                {
                    cmd.Connection = conn;
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    if (null != conn && conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }

                    throw;
                }
            }
            else
            {
                cmd.Connection = m_Trans.Connection;
                cmd.Transaction = m_Trans;
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public DataSet ExecuteDataSet()
        {
            var cmd = CreateCommand();
            return Execture(cmd, GetDataSet);
        }

        public T ExecuteScalar<T>()
        {
            var cmd = CreateCommand();
            var obj = Execture(cmd, cmd.ExecuteScalar);
            return DBConvert.To<T>(obj);
        }

        #endregion

        protected virtual DbConnection CreateConnection()
        {
            if (null == m_Factory)
            {
                throw new ArgumentNullException(nameof(DbProviderFactory));
            }

            if (string.IsNullOrWhiteSpace(m_ConnectionString))
            {
                throw new ArgumentNullException(nameof(DbConnection.ConnectionString));
            }

            var conn = m_Factory.CreateConnection();
            conn.ConnectionString = m_ConnectionString;
            return conn;
        }

        protected virtual DbCommand CreateCommand()
        {
            var cmd = m_Factory.CreateCommand();
            cmd.CommandTimeout = m_Timeout;
            cmd.Parameters.AddRange(m_Parameters.ToArray());
            cmd.CommandType = m_Type;
            cmd.CommandText = m_CommandTest;
            return cmd;
        }

        protected virtual DbDataAdapter CreateDataAdapter(DbCommand cmd)
        {
            var adapter = m_Factory.CreateDataAdapter();
            adapter.SelectCommand = cmd;
            return adapter;
        }

        protected T Execture<T>(DbCommand cmd, Func<T> exec)
        {
            if (null == m_Trans)
            {
                using (var conn = CreateConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    cmd.Connection = conn;
                    return exec();
                }
            }
            else
            {
                cmd.Connection = m_Trans.Connection;
                cmd.Transaction = m_Trans;
                return exec();
            }
        }

        protected T Execture<T>(DbCommand cmd, Func<DbCommand, T> exec)
        {
            if (null == m_Trans)
            {
                using (var conn = CreateConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    cmd.Connection = conn;
                    return exec(cmd);
                }
            }
            else
            {
                cmd.Connection = m_Trans.Connection;
                cmd.Transaction = m_Trans;
                return exec(cmd);
            }
        }

        protected DataSet GetDataSet(DbCommand cmd)
        {
            var adapter = CreateDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);

            return ds;
        }

        public const string SPResultParameterName = "@ReturnValue";

        protected List<DbParameter> m_Parameters = new List<DbParameter>();
        protected CommandType m_Type = CommandType.Text;
        protected DbProviderFactory m_Factory;
        protected int m_Timeout = ConfigConst.DefaultDALTimeout;
        protected string m_ConnectionString;
        protected string m_Cmd;
        protected DbTransaction m_Trans;
        protected string m_CommandTest;

        private readonly Queue<DataCallback> m_Callbacks = new Queue<DataCallback>();
    }
}
