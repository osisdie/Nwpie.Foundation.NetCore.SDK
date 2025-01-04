using System;
using System.Collections.Concurrent;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nwpie.Foundation.MessageQueue.SQS
{
    [Obsolete("Use MessageQueueFactory")]
    public class AwsSQSFactory : SingleCObject<AwsSQSFactory>
    {
        protected override void InitialInConstructor()
        {
            m_Serializer = new DefaultSerializer();
        }

        public ServiceResponse<IMessageQueue> GetMQService<T>(AwsSQS_Option opts)
            where T : class
        {
            var returnValue = new ServiceResponse<IMessageQueue>();
            var strKey = m_Serializer.Serialize(new
            {
                opt = opts
            }).ToMD5();

            if (false == m_MQServices.ContainsKey(strKey))
            {
                m_MQServices.TryAdd(strKey, CreateMQServiceAWS(opts));
            }

            var item = m_MQServices[strKey];
            if (null == item)
            {
                returnValue.Error(StatusCodeEnum.EmptyData, $"GetMQService internal error, kind={MessageQueueProviderEnum.SQS}, url={JsonConvert.SerializeObject(opts)}");
            }
            else
            {
                returnValue.Success().Content(item);
            }

            return returnValue;
        }

        protected IMessageQueue CreateMQServiceAWS(AwsSQS_Option option)
        {
            var returnValue = new AwsSQSClient(
                new ConfigOptions<AwsSQS_Option>(option),
                m_Serializer
            );

            if (null == returnValue)
            {
                Logger.LogError($"[{GetType().Name}] Failed to create AWS MQ service.");
            }

            return returnValue;
        }

        public override void Dispose()
        {

        }

        protected ISerializer m_Serializer;
        protected ConcurrentDictionary<string, IMessageQueue> m_MQServices = new ConcurrentDictionary<string, IMessageQueue>();
    }
}
