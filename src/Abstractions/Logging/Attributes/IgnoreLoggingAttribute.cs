using System;
using Nwpie.Foundation.Abstractions.Attributes;

namespace Nwpie.Foundation.Abstractions.Logging.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IgnoreLoggingAttribute : AttributeBase
    {
        public IgnoreLoggingAttribute() : base() { }
    }
}
