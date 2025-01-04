using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Location.Contract.Config.Refresh
{
    public class LocConfigRefresh_ResponseModel : ResultDtoBase
    {
        public Dictionary<string, bool> RawData { get; set; } = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    }
}
