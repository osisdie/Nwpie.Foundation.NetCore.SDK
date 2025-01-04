using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;

namespace Nwpie.Foundation.MessageQueue.SQS.Interfaces
{
    public interface IAwsSQSClient : IMessageQueue
    {
        /// <summary>
        /// Use DefaultTopic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IServiceResponse> PublishAsync<T>(T request);
        Task<IServiceResponse<List<T>>> ConsumeAsync<T>(bool autoAck = true, ushort? maxMessages = null);
    }
}
