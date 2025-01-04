using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IQuerySelect<TTable>
        where TTable : class
    {
        IQueryFrom<TTable> Select(params Expression<Func<TTable, object>>[] columnNameFileterExps);
    }
}
