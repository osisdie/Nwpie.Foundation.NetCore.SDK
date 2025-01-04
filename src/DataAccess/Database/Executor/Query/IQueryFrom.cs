namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQueryFrom<TTable>
        where TTable : class
    {
        IQueryFrom<TTable> Top(int count);
        IQueryFrom<TTable> Skip(int count);
        IQueryFrom<TTable> Distinct();
        IQueryCondition<TTable> From(string tableAlias);
    }
}
