using ServiceBus.Distributed.Events;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Subscriptions
{
    internal interface IEventSubscription
    {
        Task StartAsync(IHandleEvent handler, CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        void Dispose();
    }
}