namespace Nwpie.Foundation.Abstractions.MessageQueue.Interfaces
{
    public interface ICommand
    {
        void OnCommand(string topic, ICommandModel command);
    }
}
