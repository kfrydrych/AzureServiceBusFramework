using Contracts;
using ServiceBus.Distributed.Commands;
using ServiceBus.Distributed.Events;
using ServiceBus.Distributed.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SalesService
{
    public class OrderHandler : IHandleCommand<Order>
    {
        private readonly IEventBus _eventBus;

        public OrderHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task HandleAsync(Order command, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(command.Item))
                throw new BadRequestException("Ordered item must not be empty");

            Console.WriteLine("------------------");
            Console.WriteLine($"Order for {command.Item} received");
            Console.WriteLine($"Processing order {command.OrderId}...");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            var @event = new OrderAccepted
            {
                OrderId = command.OrderId,
                Item = command.Item
            };

            await _eventBus.PublishAsync(@event);
        }
    }
}
