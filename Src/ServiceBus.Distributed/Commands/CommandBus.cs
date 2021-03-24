using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Distributed.Commands
{
    internal class CommandBus : ICommandBus
    {
        private readonly string _connectionString;

        private QueueClient _queueClient;

        public CommandBus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            try
            {
                await ExecuteAsync(command);
            }
            catch (MessagingEntityNotFoundException)
            {
                await CreateQueueIfNotExistAsync(command);
                await ExecuteAsync(command);
            }
        }

        private async Task ExecuteAsync<T>(T command)
        {
            _queueClient = new QueueClient(_connectionString, typeof(T).Name);

            var data = JsonConvert.SerializeObject(command);

            var message = new Message(Encoding.UTF8.GetBytes(data));

            await _queueClient.SendAsync(message);
        }

        private async Task CreateQueueIfNotExistAsync<T>(T command)
        {
            var managerClient = new ManagementClient(_connectionString);

            if (!await managerClient.QueueExistsAsync(typeof(T).Name))
                await managerClient.CreateQueueAsync(typeof(T).Name);
        }
    }
}