namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IUpdate<TEntity>
        where TEntity : class
    {
        IUpdateSets<TEntity> UseEntityToSetValues(TEntity value);
    }
}
