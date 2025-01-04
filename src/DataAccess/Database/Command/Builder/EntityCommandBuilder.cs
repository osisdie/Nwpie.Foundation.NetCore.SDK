using System;
using System.Data;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class EntityCommandBuilder : BaseCommandBuilder
    {
        public EntityCommandBuilder(string tableAlias, OperationEnum operation)
        {
            ResolveTableAlias(tableAlias, operation, out var commandText, out var connectionString, out var provider);

            CommandConfigInfo.CommandText = commandText;
            CommandConfigInfo.ConnectionString = connectionString;
            CommandConfigInfo.Provider = provider;

            PrimaryKeyColumnProperties = ColumnPropertyCache.getPrimaryKeyColumnProperties(tableAlias);
        }

        public override ICommand Build()
        {
            ICommand command = new BaseCommand
            {
                ConnectionString = CommandConfigInfo.ConnectionString,
                CommandText = CommandConfigInfo.CommandText,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = CommandConfigInfo.CommandTimeout,
                Provider = CommandConfigInfo.Provider
            };

            if (m_IsUseEntityToSetValues)
            {
                SetParameterValues();
            }

            command.Parameters = Parameters;
            return command;
        }

        public void UseEntityToSetValues<TEntity>(TEntity entity, OperationEnum operation)
            where TEntity : class
        {
            if (null == PrimaryKeyColumnProperties &&
                0 == PrimaryKeyColumnProperties.Count)
            {
                throw new ArgumentException("There is no bigint primary key in table. ");
            }

            var propertyList = EntityHelper.ConvertEntityToPropertieList(entity);
            var hasPrimaryKey = false;

            //remove primaryKey fields for insert operation.
            if (OperationEnum.Insert == operation)
            {
                foreach (var primaryKey in PrimaryKeyColumnProperties)
                {
                    propertyList.RemoveAll(o => o.Name == primaryKey.ColumnName);
                }

                //add parameter to final collection directly
                foreach (var parameter in propertyList)
                {
                    Parameters.Add(parameter.Name, parameter.Value);
                }

                ParameterCollection.Clear();
            }
            //for the update or delete operation, we have to dependency pirmarykey field.
            else if (OperationEnum.Delete == operation)
            {
                foreach (var primaryKey in PrimaryKeyColumnProperties)
                {
                    var primaryKeyParameter = propertyList
                        .Find(o => o.Name == primaryKey.ColumnName);
                    if (null != primaryKeyParameter &&
                        false == primaryKeyParameter.Value.Equals(primaryKeyParameter.Type.GetDefault()))
                    {
                        hasPrimaryKey = true;
                        Parameters.Add(primaryKeyParameter.Name, primaryKeyParameter.Value);
                        propertyList.Remove(primaryKeyParameter);
                        break;
                    }
                }

                if (false == hasPrimaryKey)
                {
                    throw new ArgumentException("Please set primary key value. ");
                }

                ParameterCollection.Clear();
            }
            else if (operation == OperationEnum.Update)
            {
                foreach (var primaryKey in PrimaryKeyColumnProperties)
                {
                    var primaryKeyParameter = propertyList
                        .Find(o => o.Name == primaryKey.ColumnName);
                    if (null != primaryKeyParameter &&
                        false == primaryKeyParameter.Value.Equals(primaryKeyParameter.Type.GetDefault()))
                    {
                        hasPrimaryKey = true;
                        Parameters.Add(primaryKeyParameter.Name, primaryKeyParameter.Value);
                        propertyList.Remove(primaryKeyParameter);
                        break;
                    }
                }

                if (false == hasPrimaryKey)
                {
                    throw new ArgumentException("Please set primary key value. ");
                }

                //add parameter to ParameterCollection
                foreach (var property in propertyList)
                {
                    ParameterCollection.Add(property.Name, property);
                }
            }

            propertyList.Clear();
        }

        static void ResolveTableAlias(string tableAlias, OperationEnum operation, out string commandText, out string connectionString, out DataSourceEnum provider)
        {
            var array = tableAlias.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (null == array || array.Length < 2)
            {
                throw new ArgumentException($"Table alias ({tableAlias}) not specified correctly. ");
            }

            provider = ConfigManager.Instance.GetProviderByDataBaseName(array[0]);
            connectionString = ConfigManager.Instance.GetConnectionStringByDatabaseName(array[0]);
            commandText = string.Format("usp{0}{1}AutoGen",
                EnumHelper.ConvertEntityOperationEnumToString(operation),
                array.Length == 3 ? array[2] : array[1]
            );
        }
    }
}
