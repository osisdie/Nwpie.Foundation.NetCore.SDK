using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Get
{
    public class ConfigGet_ResponseModel : ResultDtoBase
    {
        public Dictionary<string, string> RawData { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
