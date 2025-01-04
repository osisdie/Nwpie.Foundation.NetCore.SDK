namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQuery<TTable> : IQuerySelect<TTable>
        where TTable : class
    {
    }
}
