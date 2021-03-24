using Contracts;
using ServiceBus.Distributed.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class SendOrderDispatchedMsgToCustomer : IHandleEvent<OrderDispatched>
    {
        public async Task HandleAsync(OrderDispatched @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"ORDER UPDATE: {@event.OrderId}.");
            Console.WriteLine($"Your order with {@event.Item} has been dispatched and should arrive within 3-5 working days.");
            Console.WriteLine("------------------");
            Console.WriteLine("");

            await Task.CompletedTask;
        }
    }
}
