using ServiceBus.Distributed.Commands;
using System;

namespace Contracts
{
    public class Order : ICommand
    {
        public Guid OrderId { get; set; }
        public string Item { get; set; }
    }
}
