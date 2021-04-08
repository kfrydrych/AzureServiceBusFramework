using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ServiceBus.Distributed.Events;
using ServiceBus.Distributed.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Subscriptions
{
    internal class BackgroundEventHandler<TEvent> : BackgroundService where TEvent : IEvent
    {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IHandleEvent<TEvent> _handler;

        public BackgroundEventHandler(IHandleEvent<TEvent> handler, ISubscriptionClient subscriptionClient)
        {
            _handler = handler;
            _subscriptionClient = subscriptionClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriptionClient.RegisterMessageHandler(async (message, token) =>
                {
                    try
                    {

                        var eventString = Encoding.UTF8.GetString(message.Body);

                        var @event = JsonConvert.DeserializeObject<TEvent>(eventString);

                        await _handler.HandleAsync(@event, token);

                        await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                    catch (BadRequestException e)
                    {
                        Console.WriteLine(e);
                        await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, e.Message, e.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        if (message.SystemProperties.DeliveryCount > 9)
                            await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, e.Message, e.ToString());
                    }


                }, new MessageHandlerOptions(args =>
                {
                    Console.WriteLine(args.Exception.Message);
                    return Task.CompletedTask;
                })
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                }
            );

            return Task.CompletedTask;
        }
    }
}
