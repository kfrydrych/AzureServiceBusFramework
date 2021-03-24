using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;

namespace ServiceBus.Distributed.Events
{
    internal class EventBus : IEventBus
    {
        private readonly string _connectionString;
        private TopicClient _topicClient;

        public EventBus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            try
            {
                await ExecuteAsync(@event);
            }
            catch (MessagingEntityNotFoundException)
            {
                await CreateTopicIfNotExistAsync(@event);
                await ExecuteAsync(@event);
            }
        }

        private async Task ExecuteAsync<T>(T @event)
        {
            _topicClient = new TopicClient(_connectionString, typeof(T).Name);

            var data = JsonConvert.SerializeObject(@event);

            var message = new Message(Encoding.UTF8.GetBytes(data));

            await _topicClient.SendAsync(message);
        }
        private async Task CreateTopicIfNotExistAsync<T>(T @event)
        {
            var managerClient = new ManagementClient(_connectionString);

            if (!await managerClient.TopicExistsAsync(typeof(T).Name))
                await managerClient.CreateTopicAsync(typeof(T).Name);
        }
    }
}
