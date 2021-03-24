using Contracts;
using Microsoft.Extensions.Hosting;
using ServiceBus.Distributed.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Configurations;

namespace ProductionCoordinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("************************************");
            Console.WriteLine(nameof(ProductionCoordinator));
            Console.WriteLine("************************************");
            Console.WriteLine("");

            using IHost host = CreateHostBuilder(args).Build();

            await host.StartListeningAsync(CancellationToken.None);

            Console.ReadLine();

            await host.StopListeningAsync(CancellationToken.None);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddServiceBus(new ServiceBusOptions(ConnectionString.Value, "Production"))
                        .EnsureEventsFromAssemblyContaining<ContractsMarker>()
                        .RegisterEventHandler<OrderAccepted, DelegateProduction>());
    }
}
