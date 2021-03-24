using Microsoft.Azure.ServiceBus;
using ServiceBus.Distributed.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Subscriptions
{
    internal class CommandSubscription<TCommand, THandler> : ICommandSubscription where TCommand : ICommand
    {
        private BackgroundCommandHandler<TCommand> _backgroundCommandHandler;
        private readonly IQueueClient _queueClient;

        public CommandSubscription(string queueName, string connectionString)
        {
            _queueClient = new QueueClient(connectionString, queueName);
        }

        public async Task StartAsync(IHandleCommand handler, CancellationToken cancellationToken)
        {
            var handlerInstance = (IHandleCommand<TCommand>)handler;

            _backgroundCommandHandler = new BackgroundCommandHandler<TCommand>(handlerInstance, _queueClient);

            await _backgroundCommandHandler.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _backgroundCommandHandler.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _backgroundCommandHandler.Dispose();
        }
    }
}
