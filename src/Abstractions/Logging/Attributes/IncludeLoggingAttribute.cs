using System;
using Nwpie.Foundation.Abstractions.Attributes;

namespace Nwpie.Foundation.Abstractions.Logging.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IncludeLoggingAttribute : AttributeBase
    {
        public IncludeLoggingAttribute() : base() { }
    }
}
