using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;

namespace Nwpie.Foundation.MessageQueue.SNS.Interfaces
{
    public interface IAwsNotificationClient : ICObject, IDisposable
    {
        /// <summary>
        /// Use DefaultTopic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IServiceResponse<List<string>>> BroadcastAsync<T>(T request);

        /// <summary>
        /// An endpoint subscribes to this SNS
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="protocol"></param>
        /// <param name="attributes"></param>
        /// <returns>SubscriptionArn</returns>
        Task<IServiceResponse<string>> SubscribeAsync(string endpoint, string protocol = "sqs", Dictionary<string, string> attributes = null);
        Task<IServiceResponse> UnsubscribeAsync(string subArn);
        Task<IServiceResponse<List<Subscription>>> ListSubscriptions(string topic = null);
        Task<IServiceResponse<string>> GetQueueUrlOrAddNewQueue(string queueName);
        Task<IServiceResponse<string>> LookupArn(string queueUrl);
        Task<IServiceResponse> TryDeleteQueue(string queueUrl);
        // (topic, message, numOfSubscriptions)
        event Action<string, string, int> BroadcastedEvent;
    }
}
