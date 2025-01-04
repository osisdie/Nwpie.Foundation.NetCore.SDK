using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Models;

namespace Nwpie.Foundation.Common.MessageQueue
{
    public abstract class MessageQueueBase : CObject, IMessageQueue
    {
        public MessageQueueBase() : base()
        {
        }

        public virtual Task<IServiceResponse> PublishAsync<T>(string topic, T request) =>
            throw new NotImplementedException();

        public virtual Task<IServiceResponse<List<T>>> ConsumeAsync<T>(string topic, bool autoAck = true, ushort? maxMessages = null) =>
            throw new NotImplementedException();

        public virtual void OnConsumed(string topic, ICommandModel message) =>
            ConsumedEvent?.Invoke(topic, message);

        public abstract void Dispose();

        public event Action<string, ICommandModel> ConsumedEvent;
    }
}
