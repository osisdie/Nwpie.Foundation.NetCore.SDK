using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Enums
{
    public enum MessageQueueProviderEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "sqs")]
        SQS = 1, //AWS

        [Display(Name = "pubsub")]
        PubSub = 2, //Google

        [Display(Name = "sse")]
        SSE = 3, // Http

        [Display(Name = "rabbitmq")]
        RabitMQ = 4,

        [Display(Name = "zeromq")]
        ZeroMQ = 5,
    };
}
