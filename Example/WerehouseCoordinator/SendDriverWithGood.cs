using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WerehouseCoordinator
{
    public class SendDriverWithGood : IHandleEvent<ProductProduced>
    {
        private readonly IEventBus _eventBus;

        public SendDriverWithGood(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task HandleAsync(ProductProduced @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"Sending driver Bob to deliver the goods to customer...");
            Console.WriteLine($"Order number: {@event.OrderId}");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            Thread.Sleep(2000);

            await _eventBus.PublishAsync(new OrderDispatched
            {
                OrderId = @event.OrderId,
                Item = @event.Item
            });
        }
    }
}
