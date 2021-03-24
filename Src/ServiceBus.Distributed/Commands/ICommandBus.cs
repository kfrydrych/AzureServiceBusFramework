using System.Threading.Tasks;

namespace ServiceBus.Distributed.Commands
{
    public interface ICommandBus
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}