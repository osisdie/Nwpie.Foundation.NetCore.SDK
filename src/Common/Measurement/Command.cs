using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Common.Measurement
{
    public class Command
    {
        public CommandType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public enum CommandType
    {
        Read = 0,
        Write = 1
    }
}
