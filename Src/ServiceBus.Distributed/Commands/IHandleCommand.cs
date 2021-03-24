using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Commands
{
    public interface IHandleCommand
    {
    }

    public interface IHandleCommand<in TCommand> : IHandleCommand
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}