using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class SendDeliveryConfirmationToCustomer : IHandleEvent<OrderDelivered>
    {
        public async Task HandleAsync(OrderDelivered @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"ORDER UPDATE: {@event.OrderId}.");
            Console.WriteLine($"Your order with {@event.Item} has been delivered. Enjoy!");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            await Task.CompletedTask;
        }
    }
}