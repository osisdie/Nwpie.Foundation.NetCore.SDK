namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IDelete<TEntity>
        where TEntity : class
    {
        IExecutor UseEntityToSetValues(TEntity value);
    }
}
