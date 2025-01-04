using System;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Attributes
{
    public class ValidationAttributeBase : ValidationAttribute
    {
        public ValidationAttributeBase()
        {
            Logger = LogMgr.CreateLogger(GetType());
            _id = Guid.NewGuid();
            _ts = DateTime.UtcNow;
        }

        public int Priority { get; set; }
        public string Description { get; set; }
        public ILogger Logger { get; private set; }
        public Guid? _id { get; private set; }
        public DateTime? _ts { get; private set; }
    }
}
