using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductionCoordinator
{
    public class DelegateProduction : IHandleEvent<OrderAccepted>
    {
        private readonly IEventBus _eventBus;

        public DelegateProduction(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task HandleAsync(OrderAccepted @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"Order for item: {@event.Item} sent to production...");
            Thread.Sleep(10000);
            Console.WriteLine($"Production completed.");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            await _eventBus.PublishAsync(new ProductProduced
            {
                OrderId = @event.OrderId,
                Item = @event.Item
            });
        }
    }
}
