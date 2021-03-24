using ServiceBus.Distributed.Subscriptions;
using System;

namespace ServiceBus.Distributed.DependencyInjection
{
    internal class CommandRegistration
    {
        public CommandRegistration(Type handlerType, ICommandSubscription commandSubscription)
        {
            HandlerType = handlerType;
            CommandSubscription = commandSubscription;
        }

        public Type HandlerType { get; }
        public ICommandSubscription CommandSubscription { get; }


    }
}