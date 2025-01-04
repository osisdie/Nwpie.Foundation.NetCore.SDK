using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class MsSqlQueryCommandBuilder
    {
        public MsSqlQueryCommandBuilder(BaseQueryCommandBuilder baseQueryCmdBuilder)
        {
            m_BaseQueryCmdBuilder = baseQueryCmdBuilder;
        }

        public ICommand Build()
        {
            var array = m_BaseQueryCmdBuilder.TableAlias.Split(new char[] { '.' });
            if (null == array || array.Length < 2)
                throw new ArgumentException($"Table alias (={m_BaseQueryCmdBuilder.TableAlias}) not specified correctly. ");

            var dbName = array[0];
            var tableName = array.Length == 3 ? string.Concat(array[1], ".", array[2]) : array[1];
            var command = new BaseCommand
            {
                Parameters = m_BaseQueryCmdBuilder.Parameters
            };

            var stringBuilder = new StringBuilder();
            BuildWhereIn(stringBuilder);
            if (m_BaseQueryCmdBuilder.TopRows <= 0 &&
                m_BaseQueryCmdBuilder.SkipRows <= 0 &&
                false == m_BaseQueryCmdBuilder.IsExecuteCount)
            {
                BuildSelect(stringBuilder);
                BuildFrom(stringBuilder, tableName);
                BuildCondition(stringBuilder, command);
                BuildOrderBy(stringBuilder);
            }
            else
            {
                stringBuilder.Append(";WITH result AS(");
                var stringBuilder2 = new StringBuilder();
                BuildOrderBy(stringBuilder2);
                BuildSelect(stringBuilder, stringBuilder2);
                BuildFrom(stringBuilder, tableName);
                BuildCondition(stringBuilder, command);
                stringBuilder.AppendLine(")");

                if (m_BaseQueryCmdBuilder.IsExecuteCount)
                {
                    stringBuilder.AppendFormat("SELECT COUNT(1) AS TotalCount FROM result", new object[0]);
                }
                else
                {
                    stringBuilder.AppendFormat("SELECT * FROM result WHERE RowNumber>{0} ORDER BY RowNumber", m_BaseQueryCmdBuilder.SkipRows);
                }
            }

            command.ConnectionString = ConfigManager.Instance.GetConnectionStringByDatabaseName(dbName);
            command.CommandText = stringBuilder.ToString();
            command.CommandType = CommandType.Text;
            command.Provider = ConfigManager.Instance.GetProviderByDataBaseName(dbName);

            return command;
        }

        #region private methods
        protected void BuildSelect(StringBuilder sb) =>
            BuildSelect(sb, null);

        protected void BuildSelect(StringBuilder sb, StringBuilder orderBy)
        {
            sb.Append("SELECT");
            if (m_BaseQueryCmdBuilder.IsDistinct)
            {
                sb.Append(" DISTINCT");
            }

            if (m_BaseQueryCmdBuilder.TopRows >= 0 &&
                false == m_BaseQueryCmdBuilder.IsExecuteCount)
            {
                var num = (m_BaseQueryCmdBuilder.SkipRows < 0) ? 0 : m_BaseQueryCmdBuilder.SkipRows;
                sb.AppendFormat(" TOP {0}", m_BaseQueryCmdBuilder.TopRows + num);
            }

            BuildSelectColumns(sb, orderBy);
        }

        protected void BuildSelectColumns(StringBuilder sb, StringBuilder orderBy)
        {
            sb.Append(" ");
            if (null == m_BaseQueryCmdBuilder.ColumnNameFilters ||
                0 == m_BaseQueryCmdBuilder.ColumnNameFilters.Length)
            {
                sb.Append("*");
            }
            else
            {
                var num = 0;
                var columnNameFilters = m_BaseQueryCmdBuilder.ColumnNameFilters;
                for (var i = 0; i < columnNameFilters.Length; i++)
                {
                    if (num >= 1)
                    {
                        sb.Append(",");
                    }

                    sb.Append(columnNameFilters[i]);
                    num++;
                }
            }

            if ((m_BaseQueryCmdBuilder.TopRows > 0 || m_BaseQueryCmdBuilder.SkipRows > 0) &&
                null != orderBy && orderBy.Length > 0)
            {
                sb.AppendFormat(",RowNumber=ROW_NUMBER() OVER ({0})", orderBy);
            }

            sb.Append(" ");
        }

        protected void BuildFrom(StringBuilder sb, string tableName)
        {
            sb.AppendFormat("FROM {0} AS T1 WITH(NOLOCK)", tableName);
        }

        protected void BuildCondition(StringBuilder sb, BaseCommand command)
        {
            if (m_BaseQueryCmdBuilder.Conditions.Count() > 0)
            {
                sb.Append(" WHERE ");
                var num = 0;
                foreach (var current in m_BaseQueryCmdBuilder.Conditions)
                {
                    var columnProperty = ColumnPropertyCache.GetColumnProperty(m_BaseQueryCmdBuilder.TableAlias, current.ColumnName);
                    if (null == columnProperty)
                    {
                        throw new Exception($"Invalid ColumnName (={current.ColumnName}) in WHERE condition. ");
                    }

                    if (num >= 1)
                    {
                        sb.Append(" AND ");
                    }

                    if (current.QueryConditionOperator != QueryConditionOperator.In)
                    {
                        switch (current.QueryConditionOperator)
                        {
                            case QueryConditionOperator.Equal:
                                sb.AppendFormat("{0}=@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.NotEqual:
                                sb.AppendFormat("{0}<>@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.LessThan:
                                sb.AppendFormat("{0}<@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.LessThanEqual:
                                sb.AppendFormat("{0}<=@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.GreaterThan:
                                sb.AppendFormat("{0}>@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.GreatThanEqual:
                                sb.AppendFormat("{0}>=@{1}", current.ColumnName, current.ParameterName);
                                break;

                            case QueryConditionOperator.Like:
                                sb.AppendFormat("{0} LIKE @{1}", current.ColumnName, current.ParameterName);
                                break;
                        }

                        command.Parameters.Add(current.ParameterName, current.Value, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                    }
                    else
                    {
                        if (current.Value is IList list && list.Count > 0)
                        {
                            if (1 == list.Count)
                            {
                                sb.AppendFormat("{0}=@{1}", current.ColumnName, current.ParameterName);
                                command.Parameters.Add(current.ParameterName, current.Value, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                            }
                            else if (list.Count <= 10000)
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
                                    var text = string.Format("@{0}_{1}", current.ParameterName, num2);
                                    sb.Append(text);
                                    command.Parameters.Add(text, current2, columnProperty.DbType, null, columnProperty.Size, columnProperty.Precision, columnProperty.Scale);
                                }
                                sb.Append(")");
                            }
                            else
                            {
                                var value = BuildInXmlParameters(list);
                                var parameterName = string.Format("@{0}Xml", current.ParameterName);
                                command.Parameters.Add(parameterName, value, DbType.Xml);
                                sb.AppendFormat("EXISTS(SELECT 1 FROM @TT_{0} AS T WHERE T.ID = T1.{0})", current.ColumnName);
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

        protected void BuildWhereIn(StringBuilder sb)
        {
            if (m_BaseQueryCmdBuilder.Conditions?.Count() > 0)
            {
                var flag = false;
                foreach (var current in m_BaseQueryCmdBuilder.Conditions)
                {
                    if (QueryConditionOperator.In == current.QueryConditionOperator &&
                        current.Value is IList list && list.Count > 20)
                    {
                        if (false == flag)
                        {
                            sb.AppendLine("DECLARE @Idoc INT");
                            flag = true;
                        }

                        sb.AppendFormat("EXEC SP_XML_PREPAREDOCUMENT @Idoc OUTPUT, @{0}Xml", current.ParameterName);
                        sb.AppendLine();
                        var columnProperty = ColumnPropertyCache.GetColumnProperty(m_BaseQueryCmdBuilder.TableAlias, current.ColumnName);
                        if (null == columnProperty)
                        {
                            throw new Exception($"Invalid ColumnName (={current.ColumnName}) in WHERE condition. ");
                        }

                        var arg = string.Empty;
                        var sqlDbType = columnProperty.SqlDbType;
                        if (sqlDbType == SqlDbType.Char)
                        {
                            arg = string.Format("{0}({1})", columnProperty.SqlDbType, columnProperty.Size);
                            sb.AppendFormat("DECLARE @TT_{0} TABLE (ID {1} PRIMARY KEY)", columnProperty.ColumnName, arg);
                            sb.AppendLine();
                            sb.AppendFormat("INSERT INTO @TT_{0}(ID) SELECT ID FROM OPENXML (@Idoc, '/Root/Item', 2) WITH (ID {1})", current.ColumnName, arg);
                            sb.AppendLine();
                            continue;
                        }

                        switch (sqlDbType)
                        {
                            case SqlDbType.NChar:
                            case SqlDbType.NVarChar:
                                arg = string.Format("{0}({1})", columnProperty.SqlDbType, columnProperty.Size);
                                sb.AppendFormat("DECLARE @TT_{0} TABLE (ID {1} PRIMARY KEY)", columnProperty.ColumnName, arg);
                                sb.AppendLine();
                                sb.AppendFormat("INSERT INTO @TT_{0}(ID) SELECT ID FROM OPENXML (@Idoc, '/Root/Item', 2) WITH (ID {1})", current.ColumnName, arg);
                                sb.AppendLine();
                                continue;

                            case SqlDbType.NText:
                                break;

                            default:
                                if (sqlDbType == SqlDbType.VarChar)
                                {
                                    arg = string.Format("{0}({1})", columnProperty.SqlDbType, columnProperty.Size);
                                    sb.AppendFormat("DECLARE @TT_{0} TABLE (ID {1} PRIMARY KEY)", columnProperty.ColumnName, arg);
                                    sb.AppendLine();
                                    sb.AppendFormat("INSERT INTO @TT_{0}(ID) SELECT ID FROM OPENXML (@Idoc, '/Root/Item', 2) WITH (ID {1})", current.ColumnName, arg);
                                    sb.AppendLine();
                                    continue;
                                }
                                break;
                        }

                        arg = columnProperty.SqlDbType.ToString();
                    }
                }
            }
        }

        protected void BuildOrderBy(StringBuilder sb)
        {
            if (m_BaseQueryCmdBuilder.OrderBys?.Count() > 0)
            {
                sb.AppendFormat(" ORDER BY ", new object[0]);
                var num = 0;
                using (var enumerator = m_BaseQueryCmdBuilder.OrderBys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (num >= 1)
                        {
                            sb.Append(",");
                        }

                        sb.Append(current.Column);
                        sb.Append($" {current.Order}");

                        num++;
                    }

                    return;
                }
            }
            if (false == m_BaseQueryCmdBuilder.IsDistinct)
            {
                var primaryKeyColumnProperties = ColumnPropertyCache.getPrimaryKeyColumnProperties(m_BaseQueryCmdBuilder.TableAlias);
                if (null != primaryKeyColumnProperties &&
                    primaryKeyColumnProperties.Count() > 0)
                {
                    sb.Append(" ORDER BY ");
                    var num2 = 0;
                    foreach (var current2 in primaryKeyColumnProperties)
                    {
                        if (num2 >= 1)
                        {
                            sb.Append(",");
                        }

                        sb.Append(current2.ColumnName);
                        num2++;
                    }
                }
            }
        }

        protected string BuildInXmlParameters(IList values)
        {
            var stringBuilder = new StringBuilder("<Root>");
            var hashSet = new HashSet<object>();
            foreach (var current in values)
            {
                if (false == hashSet.Contains(current))
                {
                    stringBuilder.AppendFormat("<Item><ID>{0}</ID></Item>", current);
                    hashSet.Add(current);
                }
            }

            stringBuilder.AppendFormat("</Root>", new object[0]);
            return stringBuilder.ToString();
        }
        #endregion

        protected BaseQueryCommandBuilder m_BaseQueryCmdBuilder { get; set; }
    }
}
