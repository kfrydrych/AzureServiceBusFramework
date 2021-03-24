using ServiceBus.Distributed.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Subscriptions
{
    internal interface ICommandSubscription
    {
        Task StartAsync(IHandleCommand handler, CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        void Dispose();
    }
}