
namespace Nwpie.Foundation.DataAccess.Database
{
    internal class DeleteExecutor<TEntity> : BaseExecutor, IDelete<TEntity>, IExecutor
        where TEntity : class
    {
        public DeleteExecutor(string tableAlias, OperationEnum operation)
            : base(tableAlias, operation) { }

        public IExecutor UseEntityToSetValues(TEntity value)
        {
            CommandBuilder.UseEntityToSetValues(value, OperationEnum.Delete);
            return this;
        }
    }
}
