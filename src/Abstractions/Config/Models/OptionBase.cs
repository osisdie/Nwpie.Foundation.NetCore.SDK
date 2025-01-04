using System;
using Nwpie.Foundation.Abstractions.Config.Interfaces;

namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class OptionBase : IOption
    {
        public DateTime? _ts { get; set; }
    }
}
