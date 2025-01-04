using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Interfaces
{
    public interface ICommandModel : IServiceResponse
    {
        string Topic { get; set; }
        string Name { get; set; }
        string Raw { get; set; }
        string PublishService { get; set; }

        /// <summary>
        /// (optional) Regex
        /// </summary>
        string FilterReceivers { get; set; }
        Dictionary<string, string> ExtensionMap { get; set; }
    }

    public interface ICommandModel<T> : IServiceResponse<T>, ICommandModel
    {

    }
}
