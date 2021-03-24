using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Events
{
    public interface IHandleEvent
    {
    }

    public interface IHandleEvent<in TEvent> : IHandleEvent where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}