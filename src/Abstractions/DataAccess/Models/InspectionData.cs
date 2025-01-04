using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.DataAccess.Models
{
    public class InspectionData
    {
        public string OriginalSqlCommand { get; set; }
        public Dictionary<string, object> OriginalParameters { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public string ReplacedSqlCommand { get; set; }
        public List<string> MissingParameters { get; set; } = new List<string>();
    }
}
