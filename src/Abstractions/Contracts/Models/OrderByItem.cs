using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;

namespace Nwpie.Foundation.Abstractions.Contracts.Models
{
    public class OrderByItem
    {
        public OrderByItem(string column, OrderByEnum order)
        {
            Column = column;
            Order = order.GetDisplayName();
        }

        public OrderByItem(string column, string order)
        {
            Column = column;
            Order = order;
        }

        public string Column { get; set; }
        public string Order { get; set; } // ASC, DESC
    }

    public static class OrderByItemExtension
    {
        public static void SetOrderBy(this OrderByItem o, OrderByEnum e) =>
            o.Order = e.GetDisplayName();
    }
}
