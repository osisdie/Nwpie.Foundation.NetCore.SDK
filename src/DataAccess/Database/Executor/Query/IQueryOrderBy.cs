using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQueryOrderBy<TTable> : IQueryExecutor<TTable>
        where TTable : class
    {
        IQueryOrderBy<TTable> OrderByAsc<TValue>(Expression<Func<TTable, TValue>> columnNameExp);
        IQueryOrderBy<TTable> OrderByDesc<TValue>(Expression<Func<TTable, TValue>> columnNameExp);
    }
}
