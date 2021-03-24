using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceBus.Distributed.Commands;

namespace ServiceBus.Distributed.Subscriptions
{
    internal class BackgroundCommandHandler<TCommand> : BackgroundService where TCommand : ICommand
    {
        private readonly IQueueClient _queueClient;
        private readonly IHandleCommand<TCommand> _handler;

        public BackgroundCommandHandler(IHandleCommand<TCommand> handler, IQueueClient queueClient)
        {
            _handler = handler;
            _queueClient = queueClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueClient.RegisterMessageHandler(async (message, token) =>
            {
                var commandString = Encoding.UTF8.GetString(message.Body);

                var command = JsonConvert.DeserializeObject<TCommand>(commandString);

                await _handler.HandleAsync(command, stoppingToken);

                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(1)
            });

            return Task.CompletedTask;
        }
    }
}