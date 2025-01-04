using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQueryCondition<TTable> : IQueryOrderBy<TTable>, IQueryExecutor<TTable>
        where TTable : class
    {
        IQueryCondition<TTable> WhereEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereNotEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereLessThan<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereLessThanEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereGreaterThan<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereGreaterThanEqual<TValue>(Expression<Func<TTable, TValue>> columnNameExp, TValue value);
        IQueryCondition<TTable> WhereIn<TValue>(Expression<Func<TTable, TValue>> columnNameExp, List<TValue> values);
        IQueryCondition<TTable> WhereLike(Expression<Func<TTable, string>> columnNameExp, string value);
    }
}
