using System.Collections.Generic;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQueryExecutor<TTable>
        where TTable : class
    {
        long ExecuteCount();
        TTable ExecuteEntity();
        IEnumerable<TTable> ExecuteEntityList();
        IEnumerable<TValue> ExecuteList<TValue>();
    }
}
