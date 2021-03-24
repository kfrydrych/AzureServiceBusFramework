namespace ServiceBus.Distributed.DependencyInjection
{
    public class ServiceBusOptions
    {
        public ServiceBusOptions(string connectionString, string serviceName)
        {
            ConnectionString = connectionString;
            ServiceName = serviceName;
        }

        public string ConnectionString { get; }
        public string ServiceName { get; }
    }
}