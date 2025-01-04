using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IUpdateSets<TEntity> : IExecutor
        where TEntity : class
    {
        IExecutor Sets(params string[] columnNameFilters);
        IUpdateCondition<TEntity> Sets(params Expression<Func<TEntity, object>>[] columnNameFilterExps);
        IUpdateSet<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> value);
    }
}
