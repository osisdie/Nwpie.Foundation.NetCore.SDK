using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    interface IQueryCommandBuilder
    {
        DynamicParameters Parameters { get; set; }
        string[] ColumnNameFilters { get; set; }
        QueryConditionCollection Conditions { get; set; }
        bool IsDistinct { get; set; }
        bool IsExecuteCount { get; set; }
        List<OrderByItem> OrderBys { get; set; }
        int SkipRows { get; set; }
        string TableAlias { get; set; }
        int TopRows { get; set; }
        ICommand Build();
    }
}
