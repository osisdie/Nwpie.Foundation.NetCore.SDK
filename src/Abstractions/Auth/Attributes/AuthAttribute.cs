using System;
using Nwpie.Foundation.Abstractions.Attributes;

namespace Nwpie.Foundation.Abstractions.Auth.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class AuthAttribute : AttributeBase
    {
        public AuthAttribute() : base() { }
    }
}
