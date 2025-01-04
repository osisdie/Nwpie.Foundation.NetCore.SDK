using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Models
{
    public class MessageModel<T> : ServiceResponse<T>, IMessageModel<T>
    {
    }
}
