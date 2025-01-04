using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Interfaces
{
    public interface IMessageQueue : ICObject, IDisposable
    {
        Task<IServiceResponse> PublishAsync<T>(string topic, T request);
        Task<IServiceResponse<List<T>>> ConsumeAsync<T>(string topic, bool autoAck = true, ushort? maxMessages = null);

        // (topic, message)
        event Action<string, ICommandModel> ConsumedEvent;
    }
}
