using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Interfaces
{
    public interface IMessageQueueFactory : IClient
    {
        ServiceResponse<IMessageQueue> GetMessageQueue(MessageQueueProviderEnum provider, IConfigOptions option);
    }
}
