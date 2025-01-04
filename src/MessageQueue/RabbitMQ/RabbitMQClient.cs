using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.MessageQueue;
using Nwpie.Foundation.MessageQueue.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nwpie.Foundation.MessageQueue.RabbitMQ
{
    public class RabbitMQClient : MessageQueueBase, IRabbitMQClient
    {
        public RabbitMQClient(IConfigOptions<RabbitMQ_Option> option, ISerializer serializer)
        {
            m_Serializer = serializer
                ?? throw new ArgumentNullException(nameof(ISerializer));
            m_Option = option
                ?? throw new ArgumentNullException(nameof(IConfigOptions<RabbitMQ_Option>));

            if (null == m_Option.Value?.Host || m_Option.Value.Port <= 0)
            {
                throw new ArgumentNullException($@"Options
                    {nameof(RabbitMQ_Option.Host)},
                    {nameof(RabbitMQ_Option.Port)} are required. ");
            }

            m_MQClient = new ConnectionFactory()
            {
                HostName = m_Option.Value.Host,
                Port = m_Option.Value.Port,
                UserName = m_Option.Value.User,
                Password = m_Option.Value.Pass,
                VirtualHost = m_Option.Value.VHost,
            };
        }

        public override async Task<IServiceResponse> PublishAsync<T>(
            string topic, T request)
        {
            var result = new ServiceResponse();

            try
            {
                var requestToJsonString = m_Serializer.Serialize(request);
                using var connection = await m_MQClient.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: topic,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var body = Encoding.UTF8.GetBytes(requestToJsonString);
                await channel.BasicPublishAsync(exchange: "",
                    routingKey: topic,
                    body: body
                );

                result.Success(StatusCodeEnum.Success, $"Finished to send topic={topic}, messageId={result.MsgId}, request={requestToJsonString}. ");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, request={request?.ToString()}, ex={ex.GetBaseFirstExceptionString()}. ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            await Task.CompletedTask;
            return result;
        }

        public override async Task<IServiceResponse<List<T>>> ConsumeAsync<T>(
            string topic, bool autoAck = true, ushort? maxMessages = null)
        {
            var result = new ServiceResponse<List<T>>()
            {
                Data = new List<T>(),
                ErrMsg = ""
            };

            try
            {
                using var connection = await m_MQClient.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                using (var signal = new ManualResetEvent(false))
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            T entry;
                            if (typeof(T) == typeof(string))
                            {
                                entry = (T)Convert.ChangeType(message, typeof(string));
                            }
                            else
                            {
                                entry = m_Serializer.Deserialize<T>(message, ignoreException: true);
                            }

                            var command = new CommandModel<T>
                            {
                                Topic = topic,
                                Raw = message,
                            }.Success().Content(entry) as ICommandModel;

                            await Task.Run(() => OnConsumed(topic, command)).ConfigureAwait(false);

                            if (null != entry)
                            {
                                result.Data.Add(entry);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.ErrMsg += $"{ex.Message}; ";
                        }

                        try
                        {
                            signal.Set();
                        }
                        catch { }
                    };

                    var maxBatchCount = m_Option.Value.MaxConsumeCount ?? maxMessages ?? 1;
                    var getMessageTimeout = m_Option.Value.GetMessageTimeout ?? 3000;
                    var getBatchTimeout = m_Option.Value.GetBatchTimeout ?? (getMessageTimeout * maxBatchCount);
                    var startTime = DateTime.UtcNow;
                    while (result.Data.Count() < maxBatchCount)
                    {
                        await channel.BasicConsumeAsync(
                            queue: topic,
                            autoAck: autoAck,
                            consumer: consumer
                        );

                        var isTimeout = false == signal.WaitOne(getMessageTimeout);
                        // cancel subscription

                        try
                        {
                            if (true == consumer.ConsumerTags?.Length > 0)
                            {
                                for (var i = 0; i < consumer.ConsumerTags.Length; ++i)
                                {
                                    await channel.BasicCancelAsync(consumer.ConsumerTags[i]);
                                }
                            }

                            if (isTimeout)
                            {
                                result.ErrMsg += $"Timeout while consuming message from topic={topic} ";
                                break;
                                //throw new TimeoutException($"Timeout while consuming message from topic={topic}");
                            }
                        }
                        catch { break; }

                        if ((DateTime.UtcNow - startTime) >= TimeSpan.FromMilliseconds(getBatchTimeout))
                        {
                            break;
                        }
                    }
                }

                result.Success(StatusCodeEnum.Success, $"Finished to consume topic={topic}, count={result.Data?.Count()}. ");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, ex={ex.GetBaseFirstExceptionString()}. ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            await Task.CompletedTask;
            return result;
        }

        public override void Dispose() { }

        protected readonly ISerializer m_Serializer;
        protected readonly ConnectionFactory m_MQClient;
        protected readonly IConfigOptions<RabbitMQ_Option> m_Option;
    }
}
