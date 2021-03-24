using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Distributed.Commands;
using ServiceBus.Distributed.Events;
using ServiceBus.Distributed.Subscriptions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.DependencyInjection
{
    public static class Extensions
    {
        private static readonly List<EventRegistration> EventRegistrations = new();
        private static readonly List<CommandRegistration> CommandRegistrations = new();

        public static RegisterBuilder AddServiceBus(this IServiceCollection services, ServiceBusOptions options)
        {
            services.AddSingleton<ICommandBus>(x => new CommandBus(options.ConnectionString));

            services.AddSingleton<IEventBus>(x => new EventBus(options.ConnectionString));

            return new RegisterBuilder(services, options);
        }
        public static RegisterBuilder EnsureEventsFromAssemblyContaining<T>(this RegisterBuilder registerBuilder)
        {
            var assembly = Assembly.GetAssembly(typeof(T));

            var type = typeof(IEvent);

            var events = assembly.GetTypes().Where(x => type.IsAssignableFrom(x) && x.IsAbstract == false);

            var managerClient = new ManagementClient(registerBuilder.Options.ConnectionString);

            foreach (var @event in events)
            {
                var topicExist = managerClient.TopicExistsAsync(@event.Name).Result;

                if (!topicExist)
                    managerClient.CreateTopicAsync(@event.Name);
            }

            return registerBuilder;
        }
        public static RegisterBuilder RegisterEventHandler<TEvent, THandler>(this RegisterBuilder registerBuilder, string subscriptionName = null) where TEvent : IEvent
        {
            registerBuilder.ServiceCollection.AddTransient(typeof(IHandleEvent<TEvent>), typeof(THandler));

            var appName = registerBuilder.Options.ServiceName;

            var subName = subscriptionName ?? $"{appName}_{typeof(THandler).Name}";

            var subscription = EventSubscription<TEvent, THandler>.Create(subName, registerBuilder.Options.ConnectionString).Result;

            EventRegistrations.Add(new EventRegistration(typeof(IHandleEvent<TEvent>), subscription));

            return registerBuilder;
        }
        public static RegisterBuilder RegisterCommandHandler<TCommand, THandler>(this RegisterBuilder registerBuilder, string queueName = null) where TCommand : ICommand
        {
            registerBuilder.ServiceCollection.AddTransient(typeof(IHandleCommand<TCommand>), typeof(THandler));

            var queue = queueName ?? typeof(TCommand).Name;

            var subscription = new CommandSubscription<TCommand, THandler>(queue, registerBuilder.Options.ConnectionString);

            CommandRegistrations.Add(new CommandRegistration(typeof(IHandleCommand<TCommand>), subscription));

            return registerBuilder;
        }

        // IApplicationBuilder
        public static async Task StartListeningAsync(this IApplicationBuilder app, CancellationToken cancellationToken)
        {
            foreach (var registration in EventRegistrations)
            {
                var handler = (IHandleEvent)app.ApplicationServices.GetService(registration.HandlerType);

                var subscription = registration.EventSubscription;

                await subscription.StartAsync(handler, cancellationToken);
            }

            foreach (var registration in CommandRegistrations)
            {
                var handler = (IHandleCommand)app.ApplicationServices.GetService(registration.HandlerType);

                var subscription = registration.CommandSubscription;

                await subscription.StartAsync(handler, cancellationToken);
            }
        }
        public static async Task StopListeningAsync(this IApplicationBuilder app, CancellationToken cancellationToken)
        {
            foreach (var registration in EventRegistrations)
            {
                var subscription = registration.EventSubscription;

                await subscription.StopAsync(cancellationToken);
            }

            foreach (var registration in CommandRegistrations)
            {
                var subscription = registration.CommandSubscription;

                await subscription.StopAsync(cancellationToken);
            }
        }
        public static void Dispose(this IApplicationBuilder app)
        {
            foreach (var registration in EventRegistrations)
            {
                var subscription = registration.EventSubscription;

                subscription.Dispose();
            }

            foreach (var registration in CommandRegistrations)
            {
                var subscription = registration.CommandSubscription;

                subscription.Dispose();
            }
        }

        // IHost
        public static async Task StartListeningAsync(this IHost app, CancellationToken cancellationToken)
        {
            foreach (var registration in EventRegistrations)
            {
                var handler = (IHandleEvent)app.Services.GetService(registration.HandlerType);

                var subscription = registration.EventSubscription;

                await subscription.StartAsync(handler, cancellationToken);
            }

            foreach (var registration in CommandRegistrations)
            {
                var handler = (IHandleCommand)app.Services.GetService(registration.HandlerType);

                var subscription = registration.CommandSubscription;

                await subscription.StartAsync(handler, cancellationToken);
            }
        }
        public static async Task StopListeningAsync(this IHost app, CancellationToken cancellationToken)
        {
            foreach (var registration in EventRegistrations)
            {
                var subscription = registration.EventSubscription;

                await subscription.StopAsync(cancellationToken);
            }

            foreach (var registration in CommandRegistrations)
            {
                var subscription = registration.CommandSubscription;

                await subscription.StopAsync(cancellationToken);
            }
        }
        public static void Dispose(this IHost app)
        {
            foreach (var registration in EventRegistrations)
            {
                var subscription = registration.EventSubscription;

                subscription.Dispose();
            }

            foreach (var registration in CommandRegistrations)
            {
                var subscription = registration.CommandSubscription;

                subscription.Dispose();
            }
        }
    }
}
