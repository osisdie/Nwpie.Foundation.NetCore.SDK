using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Enums;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class Query<TTable> : IQuery<TTable>, IQuerySelect<TTable>, IQueryFrom<TTable>, IQueryCondition<TTable>, IQueryOrderBy<TTable>, IQueryExecutor<TTable>
        where TTable : class
    {
        #region implement IQuerySelect interface
        public IQueryFrom<TTable> Select(params Expression<Func<TTable, object>>[] columnNameFileterExps)
        {
            m_CommandBuilder.ColumnNameFilters = PropertyMapper.GetPropertyMapNamesFromExps<TTable>(columnNameFileterExps);
            return this;
        }
        #endregion

        #region IQueryFrom Interface
        public IQueryFrom<TTable> Top(int count)
        {
            m_CommandBuilder.TopRows = count;
            return this;
        }

        public IQueryFrom<TTable> Skip(int count)
        {
            m_CommandBuilder.SkipRows = count;
            return this;
        }

        public IQueryFrom<TTable> Distinct()
        {
            m_CommandBuilder.IsDistinct = true;
            return this;
        }

        public IQueryCondition<TTable> From(string tableAlias)
        {
            m_CommandBuilder.TableAlias = tableAlias;
            return this;
        }
        #endregion

        #region implement IQueryCondition interface
        public IQueryCondition<TTable> WhereEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.Equal, value));
            return this;
        }

        public IQueryCondition<TTable> WhereNotEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.NotEqual, value));
            return this;
        }

        public IQueryCondition<TTable> WhereLessThan<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.LessThan, value));
            return this;
        }

        public IQueryCondition<TTable> WhereLessThanEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.LessThanEqual, value));
            return this;
        }

        public IQueryCondition<TTable> WhereGreaterThan<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.GreaterThan, value));
            return this;
        }

        public IQueryCondition<TTable> WhereGreaterThanEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.GreatThanEqual, value));
            return this;
        }

        public IQueryCondition<TTable> WhereIn<TValue>(Expression<Func<TTable, TValue>> columnNameExp, List<TValue> values)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.In, values));
            return this;
        }
        public IQueryCondition<TTable> WhereLike(Expression<Func<TTable, string>> columnNameExp, string value)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, string>(columnNameExp);
            m_CommandBuilder.Conditions.Add(new QueryCondition(propertyMapNameFromExp, QueryConditionOperator.Like, value));
            return this;
        }
        #endregion

        #region implement IQueryOrderBy interface
        public IQueryOrderBy<TTable> OrderByAsc<TValue>(Expression<Func<TTable, TValue>> columnNameExp)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.OrderBys.Add(new OrderByItem(propertyMapNameFromExp, OrderByEnum.Ascending));
            return this;
        }
        public IQueryOrderBy<TTable> OrderByDesc<TValue>(Expression<Func<TTable, TValue>> columnNameExp)
        {
            var propertyMapNameFromExp = PropertyMapper.GetPropertyMapNameFromExp<TTable, TValue>(columnNameExp);
            m_CommandBuilder.OrderBys.Add(new OrderByItem(propertyMapNameFromExp, OrderByEnum.Descending));
            return this;
        }
        #endregion

        #region implement IQueryExecutor interface
        public long ExecuteCount()
        {
            m_CommandBuilder.IsExecuteCount = true;
            var command = m_CommandBuilder.Build();
            var result = command.ExecuteScalarAsync<long>().ConfigureAwait(false).GetAwaiter().GetResult();
            m_CommandBuilder.IsExecuteCount = false;

            return result;
        }

        public TTable ExecuteEntity()
        {
            m_CommandBuilder.TopRows = 1;
            var command = m_CommandBuilder.Build();
            return command.ExecuteEntityAsync<TTable>().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public IEnumerable<TTable> ExecuteEntityList()
        {
            var command = m_CommandBuilder.Build();
            return command.ExecuteEntityListAsync<TTable>().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public IEnumerable<TValue> ExecuteList<TValue>()
        {
            var command = m_CommandBuilder.Build();
            return command.ExecuteListAsync<TValue>().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        #endregion

        private readonly IQueryCommandBuilder m_CommandBuilder = new BaseQueryCommandBuilder();
    }
}
