using Configurations;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Distributed.Commands;
using ServiceBus.Distributed.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerOne
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;


            var commandBus = provider.GetService<ICommandBus>();

            while (true)
            {
                Console.WriteLine($"Please type would you like to order? [E]xit");
                var item = Console.ReadLine();

                if (item?.ToLower() == "e")
                    break;

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    Item = item
                };


                Console.WriteLine($"---------------------------------------");
                Console.WriteLine($"Order for {order.Item} submitted.");
                Console.WriteLine($"Ref: {order.OrderId}");
                Console.WriteLine($"---------------------------------------");

                Thread.Sleep(2000);

                await commandBus.SendAsync(order);

                Console.Clear();
            }

            Console.WriteLine($"Connection closed");
            Console.ReadLine();
        }


        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddServiceBus(new ServiceBusOptions(ConnectionString.Value, "EShop"))
                        .EnsureEventsFromAssemblyContaining<ContractsMarker>());
    }
}
