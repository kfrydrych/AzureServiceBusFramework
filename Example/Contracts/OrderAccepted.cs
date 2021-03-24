using ServiceBus.Distributed.Events;
using System;

namespace Contracts
{
    public class OrderAccepted : IEvent
    {
        public Guid OrderId { get; set; }
        public string Item { get; set; }
    }
}
