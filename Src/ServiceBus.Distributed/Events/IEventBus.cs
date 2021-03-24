using System.Threading.Tasks;

namespace ServiceBus.Distributed.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}