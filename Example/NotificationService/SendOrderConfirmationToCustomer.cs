using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class SendOrderConfirmationToCustomer : IHandleEvent<OrderAccepted>
    {
        public async Task HandleAsync(OrderAccepted @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"This is a confirmation of buying {@event.Item} from us.");
            Console.WriteLine($"The order number for your purchase is: {@event.OrderId}.");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            await Task.CompletedTask;
        }
    }
}
