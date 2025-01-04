using System;
using System.Collections.Concurrent;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.MessageQueue.RabbitMQ;
using Nwpie.Foundation.MessageQueue.SQS;

namespace Nwpie.Foundation.MessageQueue.Factory
{
    public class MessageQueueFactory : CObject, IMessageQueueFactory
    {
        public MessageQueueFactory(ISerializer serializer)
        {
            m_Serializer = serializer ??
                throw new ArgumentNullException(nameof(ISerializer));
        }

        public ServiceResponse<IMessageQueue> GetMessageQueue(MessageQueueProviderEnum provider, IConfigOptions option)
        {
            if (null == option as IConfigOptions<AwsSQS_Option> &&
                null == option as IConfigOptions<RabbitMQ_Option>)
            {
                throw new ArgumentNullException(nameof(IConfigOptions));
            }

            var returnValue = new ServiceResponse<IMessageQueue>();
            var strKey = option.ToString();

            if (false == m_MQServiceMap.ContainsKey(strKey))
            {
                switch (provider)
                {
                    case MessageQueueProviderEnum.SQS:
                        {
                            IMessageQueue sqs = new AwsSQSClient(option as IConfigOptions<AwsSQS_Option>, m_Serializer);
                            m_MQServiceMap.TryAdd(strKey, sqs);
                            break;
                        }

                    case MessageQueueProviderEnum.RabitMQ:
                        {
                            IMessageQueue sqs = new RabbitMQClient(option as IConfigOptions<RabbitMQ_Option>, m_Serializer);
                            m_MQServiceMap.TryAdd(strKey, sqs);
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }

            returnValue.Data = m_MQServiceMap[strKey];
            returnValue.IsSuccess = null != returnValue.Data;

            return returnValue;
        }

        public void Dispose()
        {
            m_MQServiceMap?.Clear();
        }

        protected readonly ISerializer m_Serializer;
        protected readonly ConcurrentDictionary<string, IMessageQueue> m_MQServiceMap = new ConcurrentDictionary<string, IMessageQueue>(StringComparer.OrdinalIgnoreCase);
    }
}
