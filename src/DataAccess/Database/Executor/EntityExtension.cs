
using Nwpie.Foundation.Abstractions.DataAccess.Interfaces;

namespace Nwpie.Foundation.DataAccess.Database
{
    public static class EntityExtension
    {
        public static IExecutor Insert<TEntity>(this TEntity entity, string tableAlias)
            where TEntity : class, IEntity
        {
            var executor = new InsertExecutor<TEntity>(tableAlias, OperationEnum.Insert);
            return executor.UseEntityToSetValues(entity);
        }

        public static IExecutor Delete<TEntity>(this TEntity entity, string tableAlias)
            where TEntity : class, IEntity
        {
            var executor = new DeleteExecutor<TEntity>(tableAlias, OperationEnum.Delete);
            return executor.UseEntityToSetValues(entity);
        }

        public static IUpdateSets<TEntity> Update<TEntity>(this TEntity entity, string tableAlias)
            where TEntity : class, IEntity
        {
            var executor = new UpdateExecutor<TEntity>(tableAlias, OperationEnum.Update);
            return executor.UseEntityToSetValues(entity);
        }
    }
}
