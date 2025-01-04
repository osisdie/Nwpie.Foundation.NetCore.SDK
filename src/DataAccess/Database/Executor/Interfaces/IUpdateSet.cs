using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IUpdateSet<TEntity>
        where TEntity : class
    {
        IUpdateSet<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> value);

        IUpdateCondition<TEntity> End();
    }
}
