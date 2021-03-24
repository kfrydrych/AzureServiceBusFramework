using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DriverMobileDevice
{
    public class PickAndTakeToCustomer : IHandleEvent<OrderDispatched>
    {
        private readonly IEventBus _eventBus;

        public PickAndTakeToCustomer(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task HandleAsync(OrderDispatched @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"Picking order: {@event.OrderId} | {@event.Item}...");
            Thread.Sleep(1000);
            Console.WriteLine("Leaving warehouse...");
            Thread.Sleep(10000);
            Console.WriteLine("Arrived to customer, scanning barcode...");
            Thread.Sleep(1000);
            Console.WriteLine("------------------");
            Console.WriteLine("");

            await _eventBus.PublishAsync(new OrderDelivered
            {
                OrderId = @event.OrderId,
                Item = @event.Item
            });
        }
    }
}
