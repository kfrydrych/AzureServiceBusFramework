using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ServiceBus.Distributed.Commands;
using ServiceBus.Distributed.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                try
                {
                    var commandString = Encoding.UTF8.GetString(message.Body);

                    var command = JsonConvert.DeserializeObject<TCommand>(commandString);

                    await _handler.HandleAsync(command, token);

                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
                catch (BadRequestException e)
                {
                    Console.WriteLine(e);
                    await _queueClient.DeadLetterAsync(message.SystemProperties.LockToken, e.Message, e.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (message.SystemProperties.DeliveryCount > 9)
                        await _queueClient.DeadLetterAsync(message.SystemProperties.LockToken, e.Message, e.ToString());
                }

            }, new MessageHandlerOptions(args =>
            {
                Console.WriteLine(args.Exception.Message);
                return Task.CompletedTask;
            })
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            });

            return Task.CompletedTask;
        }
    }
}