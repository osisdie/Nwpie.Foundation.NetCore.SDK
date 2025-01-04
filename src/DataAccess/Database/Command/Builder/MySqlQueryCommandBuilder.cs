using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class MySqlQueryCommandBuilder
    {
        public MySqlQueryCommandBuilder(BaseQueryCommandBuilder baseQueryCmdBuilder)
        {
            m_baseQueryCmdBuilder = baseQueryCmdBuilder;
        }

        public ICommand Build()
        {
            var array = m_baseQueryCmdBuilder.TableAlias.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (null == array || array.Length < 2)
            {
                throw new ArgumentException($"Table alias (={m_baseQueryCmdBuilder.TableAlias}) not specified correctly. ");
            }

            var dbName = array[0];
            var tableName = array.Length == 3 ? string.Concat(array[1], ".", array[2]) : array[1];
            var command = new BaseCommand
            {
                Parameters = m_baseQueryCmdBuilder.Parameters
            };

            var stringBuilder = new StringBuilder();
            BuildSelect(stringBuilder, tableName);
            BuildWhere(stringBuilder, command);
            BuildOrderBy(stringBuilder);

            if (m_baseQueryCmdBuilder.TopRows > 0 &&
                m_baseQueryCmdBuilder.SkipRows >= 0 &&
                false == m_baseQueryCmdBuilder.IsExecuteCount)
            {
                BuildLimit(stringBuilder);
            }

            if (m_baseQueryCmdBuilder.IsExecuteCount)
            {
                BuildGetCount(stringBuilder);
            }

            command.ConnectionString = ConfigManager.Instance.GetConnectionStringByDatabaseName(dbName);
            command.CommandText = stringBuilder.ToString();
            command.CommandType = CommandType.Text;
            command.Provider = ConfigManager.Instance.GetProviderByDataBaseName(dbName);

            return command;
        }

        #region
        protected void BuildSelect(StringBuilder sb, string tableName)
        {
            sb.Append("select ");
            if (m_baseQueryCmdBuilder.IsDistinct)
            {
                sb.Append("distinct ");
            }

            if (null != m_baseQueryCmdBuilder.ColumnNameFilters &&
                m_baseQueryCmdBuilder.ColumnNameFilters.Length > 0)
            {
                for (var i = 0; i < m_baseQueryCmdBuilder.ColumnNameFilters.Length; i++)
                {
                    var value = m_baseQueryCmdBuilder.ColumnNameFilters[i];
                    if (i > 0)
                    {
                        sb.Append(',');
                    }

                    sb.Append(value);
                }
            }
            else
            {
                sb.Append('*');
            }

            sb.AppendFormat(" from {0} ", tableName);
        }

        protected void BuildWhere(StringBuilder sb, BaseCommand command)
        {
            if (m_baseQueryCmdBuilder.Conditions?.Count() > 0)
            {
                sb.Append(" WHERE ");
                var num = 0;
                foreach (var current in m_baseQueryCmdBuilder.Conditions)
                {
                    var columnProperty = ColumnPropertyCache.GetColumnProperty(m_baseQueryCmdBuilder.TableAlias, current.ColumnName);
                    if (null == columnProperty)
                    {
                        throw new Exception($"Invalid ColumnName (={current.ColumnName}) in WHERE condition. ");
                    }

                    if (num >= 1)
                    {
                        sb.Append(" AND ");
                    }

                    if (QueryConditionOperator.In != current.QueryConditionOperator)
                    {
                        switch (current.QueryConditionOperator)
                        {
                            case QueryConditionOperator.Equal:
                                sb.AppendFormat("{0}={2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.NotEqual:
                                sb.AppendFormat("{0}<>{2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.LessThan:
                                sb.AppendFormat("{0}<{2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.LessThanEqual:
                                sb.AppendFormat("{0}<={2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.GreaterThan:
                                sb.AppendFormat("{0}>{2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.GreatThanEqual:
                                sb.AppendFormat("{0}>={2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                            case QueryConditionOperator.Like:
                                sb.AppendFormat("{0} LIKE {2}{1}", current.ColumnName, current.ParameterName, "?");
                                break;
                        }

                        command.Parameters.Add(current.ParameterName, current.Value, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                    }
                    else
                    {
                        var list = current.Value as IList;
                        if (null != list && list.Count > 0)
                        {
                            if (1 == list.Count)
                            {
                                sb.AppendFormat("{0}={2}{1}", current.ColumnName, current.ParameterName, "?");
                                command.Parameters.Add(current.ParameterName, current.Value, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                            }
                            else
                            {
                                if (list.Count <= 10000)
                                {
                                    sb.AppendFormat("{0} IN(", current.ColumnName);
                                    var num2 = 0;
                                    foreach (var current2 in list)
                                    {
                                        if (num2 >= 1)
                                        {
                                            sb.Append(",");
                                        }

                                        num2++;
                                        var text = string.Format("{2}{0}_{1}", current.ParameterName, num2, "?");
                                        sb.Append(text);
                                        command.Parameters.Add(text, current2, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                                    }

                                    sb.Append(")");
                                }
                                else
                                {
                                    throw new Exception($"Invalid ColumnName (={current.ColumnName}) in WHERE condition. In list count greater than 100. ");
                                }
                            }
                        }
                        else
                        {
                            sb.Append("1=0");
                        }
                    }

                    num++;
                }
            }
        }

        protected void BuildOrderBy(StringBuilder sb)
        {
            if (m_baseQueryCmdBuilder.OrderBys?.Count() > 0)
            {
                sb.AppendFormat(" ORDER BY ", new object[0]);
                var num = 0;
                foreach (var current in m_baseQueryCmdBuilder.OrderBys)
                {
                    if (num >= 1) { sb.Append(","); }

                    sb.Append(current.Column);
                    sb.Append($" {current.Order}");

                    num++;
                }
            }
            else
            {
                if (false == m_baseQueryCmdBuilder.IsDistinct)
                {
                    var primaryKeyColumnProperties = ColumnPropertyCache.getPrimaryKeyColumnProperties(m_baseQueryCmdBuilder.TableAlias);
                    if (null != primaryKeyColumnProperties &&
                        primaryKeyColumnProperties.Count() > 0)
                    {
                        sb.Append(" ORDER BY ");
                        var num = 0;
                        foreach (var current2 in primaryKeyColumnProperties)
                        {
                            if (num >= 1)
                            {
                                sb.Append(",");
                            }

                            sb.Append(current2.ColumnName);
                            num++;
                        }
                    }
                }
            }
        }

        protected void BuildLimit(StringBuilder sb) =>
            sb.AppendFormat(" limit {0},{1} ", m_baseQueryCmdBuilder.SkipRows, m_baseQueryCmdBuilder.TopRows);

        protected void BuildGetCount(StringBuilder sb)
        {
            sb.Insert(0, "select count(0) as RowsCount from (");
            sb.Append(") tempCountT");
        }
        #endregion

        protected BaseQueryCommandBuilder m_baseQueryCmdBuilder { get; set; }
    }
}
