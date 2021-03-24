using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceBus.Distributed.Events;

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
                    var eventString = Encoding.UTF8.GetString(message.Body);

                    var @event = JsonConvert.DeserializeObject<TEvent>(eventString);

                    await _handler.HandleAsync(@event, stoppingToken);

                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

                }, new MessageHandlerOptions(args => Task.CompletedTask)
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                }
            );

            return Task.CompletedTask;
        }
    }
}
