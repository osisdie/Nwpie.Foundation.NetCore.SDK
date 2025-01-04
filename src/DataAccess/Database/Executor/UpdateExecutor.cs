using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class UpdateExecutor<TEntity> : BaseExecutor, IUpdate<TEntity>, IUpdateSets<TEntity>, IUpdateSet<TEntity>, IUpdateCondition<TEntity>, IExecutor
        where TEntity : class
    {
        public UpdateExecutor(string tableAlias, OperationEnum operation)
            : base(tableAlias, operation)
        {
        }

        public IUpdateCondition<TEntity> End() => this;

        public IUpdateSets<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> value)
        {
            CommandBuilder.SetParameterValue(value);
            return this;
        }

        public IUpdateSets<TEntity> UseEntityToSetValues(TEntity value)
        {
            CommandBuilder.UseEntityToSetValues(value, OperationEnum.Update);
            return this;
        }

        public IUpdateCondition<TEntity> Sets(params Expression<Func<TEntity, object>>[] columnNameFilterExps)
        {
            CommandBuilder.SetParameterValues(columnNameFilterExps);
            return this;
        }

        public IExecutor Sets(params string[] columnNameFilters)
        {
            CommandBuilder.SetParameterValues(columnNameFilters);
            return this;
        }

        public IUpdateCondition<TEntity> Where<TValue>(Expression<Func<TEntity, TValue>> value)
        {
            CommandBuilder.SetParameterValue(value);
            return this;
        }

        IExecutor IUpdateCondition<TEntity>.End() => this;

        IUpdateSet<TEntity> IUpdateSets<TEntity>.Set<TValue>(Expression<Func<TEntity, TValue>> value)
        {
            CommandBuilder.SetParameterValue(value);
            return this;
        }

        IUpdateSet<TEntity> IUpdateSet<TEntity>.Set<TValue>(Expression<Func<TEntity, TValue>> value)
        {
            CommandBuilder.SetParameterValue(value);
            return this;
        }
    }
}
