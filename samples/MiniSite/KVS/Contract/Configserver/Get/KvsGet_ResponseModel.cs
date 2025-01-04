using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Get
{
    public class KvsGet_ResponseModel : ResultDtoBase
    {
        public Dictionary<string, string> RawData { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
