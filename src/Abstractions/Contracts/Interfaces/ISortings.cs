using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface ISortings
    {
        List<OrderByItem> Sorts { get; set; }
    }
}
