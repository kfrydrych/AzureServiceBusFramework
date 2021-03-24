using ServiceBus.Distributed.Subscriptions;
using System;

namespace ServiceBus.Distributed.DependencyInjection
{
    internal class EventRegistration
    {
        public EventRegistration(Type handlerType, IEventSubscription eventSubscription)
        {
            HandlerType = handlerType;
            EventSubscription = eventSubscription;
        }

        public Type HandlerType { get; }
        public IEventSubscription EventSubscription { get; }
    }
}
