using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBus.Distributed.Events;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Subscriptions
{
    internal class EventSubscription<TEvent, THandler> : IEventSubscription where TEvent : IEvent
    {
        private BackgroundEventHandler<TEvent> _backgroundEventHandler;
        private readonly ISubscriptionClient _client;

        private EventSubscription(ISubscriptionClient client)
        {
            _client = client;
        }

        public static async Task<EventSubscription<TEvent, THandler>> Create(string subscriptionName, string connectionString)
        {
            var client = await EnsureSubscriptionFor(subscriptionName, connectionString);

            return new EventSubscription<TEvent, THandler>(client);
        }

        public async Task StartAsync(IHandleEvent handler, CancellationToken cancellationToken)
        {
            var handlerInstance = (IHandleEvent<TEvent>)handler;

            _backgroundEventHandler = new BackgroundEventHandler<TEvent>(handlerInstance, _client);

            await _backgroundEventHandler.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _backgroundEventHandler.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _backgroundEventHandler.Dispose();
        }

        private static async Task<ISubscriptionClient> EnsureSubscriptionFor(string subscriptionName, string connectionString)
        {
            var topicPath = typeof(TEvent).Name;

            var client = new ManagementClient(connectionString);

            if (!await client.SubscriptionExistsAsync(topicPath, subscriptionName))
                await client.CreateSubscriptionAsync(topicPath, subscriptionName);

            return new SubscriptionClient(connectionString, topicPath, subscriptionName);
        }
    }
}