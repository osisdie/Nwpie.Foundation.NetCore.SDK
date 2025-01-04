using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Models
{
    public class CommandModel<T> : ServiceResponse<T>, ICommandModel<T>
    {
        public string Topic { get; set; }
        public string Name { get; set; }
        public string Raw { get; set; }
        public string PublishService { get; set; }

        /// <summary>
        /// (optional) Regex
        /// </summary>
        public string FilterReceivers { get; set; }
        public Dictionary<string, string> ExtensionMap { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public class CommandModel : CommandModel<string>
    {
    }
}
