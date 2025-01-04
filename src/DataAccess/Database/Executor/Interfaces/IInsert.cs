namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IInsert<TEntity>
        where TEntity : class
    {
        IExecutor UseEntityToSetValues(TEntity value);
    }
}
