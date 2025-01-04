using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Notification.Extensions;
using Nwpie.Foundation.MessageQueue.RabbitMQ;
using RabbitMQ.Client;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.MessageQueue
{
    [Collection("MessageQueue_RabbitMQ_Test")]
    public class MQ_RabbitMQ_Test : TestBase
    {
        public MQ_RabbitMQ_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Make sure RabbitMQ is connective")]
        public async Task Integration_Test()
        {
            await Health_Test();
            await Client_Test();
        }

        async Task Health_Test()
        {
            var opt = GetDevOption();
            Assert.NotNull(opt);

            var factory = new ConnectionFactory()
            {
                //HostName = "127.0.0.1",
                //HostName = "172.18.140.253", // WSL2 IP
                HostName = opt.Host,
                Port = opt.Port,
                UserName = opt.User,
                Password = opt.Pass,
                VirtualHost = opt.VHost ?? "/"
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                {
                    await channel.QueueDeclareAsync(queue: EchoTopic,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var body = Encoding.UTF8.GetBytes("World");

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: NotificationTopic,
                        body: body
                    );
                }
                Assert.NotNull(channel);
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        [Fact(Skip = "sudo service rabbitmq-server status")]
        public async Task LocalConnect_Test()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "127.0.0.1",
                    //HostName = "172.18.140.253", // WSL2 IP
                    Port = 5672,
                    UserName = "dev",
                    Password = "**",
                    VirtualHost = "/",
                    Ssl = new SslOption
                    {
                        Enabled = false
                    }
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                Console.WriteLine("Connected to RabbitMQ!");
                Assert.NotNull(channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Assert.Null(ex);
            }
        }

        async Task Client_Test()
        {
            var opt = GetDevOption();
            Assert.NotNull(opt);

            m_Option = new ConfigOptions<RabbitMQ_Option>(opt);
            var topic = "unittest";
            var client = new RabbitMQClient(m_Option, Serializer);
            client.ConsumedEvent += OnConsumed;

            var request = new NotifySend_RequestModel()
            {
                Kind = (byte)NotifyChannelEnum.Email,
                Title = $"{ServiceContext.ApiName}-{MethodBase.GetCurrentMethod()}-UnitTest",
                Message = "Awesome !",
                ToList = "dev@kevinw.net",
                Sender = client.GetType().ToString()
            }.PreProcess();

            var publishResult = await client.PublishAsync(topic, request);
            Assert.True(publishResult.IsSuccess);

            {
                var consumeResult = await client.ConsumeAsync<NotifySend_RequestModel>(topic, true, 1);
                Assert.True(consumeResult.IsSuccess);
                Assert.NotEmpty(consumeResult.Data);
            }
        }

        void OnConsumed(string topic, ICommandModel message)
        {
            Console.WriteLine($"received from topic(={topic}), msg={message?.Raw}");
        }

        RabbitMQ_Option GetDevOption()
        {
            return new RabbitMQ_Option()
            {
                Host = "127.0.0.1",
                //Host = "localhost",
                //Host = "172.18.140.253", // WSL2 IP
                //Host = "192.168.182.55",
                Port = 5672,
                User = "dev",
                Pass = "**",
                VHost = "/"
            };
        }

        private const string EchoTopic = "echo";
        private const string NotificationTopic = "unittest";

        protected ConfigOptions<RabbitMQ_Option> m_Option;
    }
}
