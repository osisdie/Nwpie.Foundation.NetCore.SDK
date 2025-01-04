
namespace Nwpie.Foundation.DataAccess.Database
{
    internal class InsertExecutor<TEntity> : BaseExecutor, IInsert<TEntity>, IExecutor
        where TEntity : class
    {
        public InsertExecutor(string tableAlias, OperationEnum operation)
            : base(tableAlias, operation) { }

        public IExecutor UseEntityToSetValues(TEntity value)
        {
            CommandBuilder.UseEntityToSetValues(value, OperationEnum.Insert);
            return this;
        }
    }
}
