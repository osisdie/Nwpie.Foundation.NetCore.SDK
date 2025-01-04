using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IRequestDto
    {
        int? PageIndex { get; set; }
        int? PageSize { get; set; }

        List<OrderByItem> Sorts { get; set; }
        List<string> Fields { get; set; }
        Dictionary<string, string> ExtensionMap { get; set; }

        bool Validate();
        bool ValidateAndThrow();
    }



}
