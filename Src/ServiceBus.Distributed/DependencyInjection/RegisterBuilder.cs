using Microsoft.Extensions.DependencyInjection;

namespace ServiceBus.Distributed.DependencyInjection
{
    public class RegisterBuilder
    {
        internal readonly ServiceBusOptions Options;
        internal readonly IServiceCollection ServiceCollection;

        public RegisterBuilder(IServiceCollection serviceCollection, ServiceBusOptions options)
        {
            Options = options;
            ServiceCollection = serviceCollection;
        }
    }
}