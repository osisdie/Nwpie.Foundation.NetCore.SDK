using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IUpdateCondition<TEntity> : IExecutor
        where TEntity : class
    {
        IUpdateCondition<TEntity> Where<TValue>(Expression<Func<TEntity, TValue>> value);

        IExecutor End();
    }
}
