using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Interfaces;
using System.Text.Json;

namespace FCG_Users.Infrastructure.Users.Events
{
    public class ServiceBusEventPublisher : IEventPublisher
    {
        private readonly ServiceBusClient _client;
        private readonly string _queueName;

        public ServiceBusEventPublisher(ServiceBusClient client, string queueName)
        {
            _client = client;
            _queueName = queueName;
        }

        public async Task PublishAsync<T>(T evt, string subject, string correlationId)
        {
            var sender = _client.CreateSender(_queueName);
            var body = JsonSerializer.Serialize(evt);
            var message = new ServiceBusMessage(body)
            {
                ContentType = "application/json",
                Subject = subject,
                CorrelationId = correlationId
            };

            message.ApplicationProperties["EventName"] = subject;
            message.ApplicationProperties["OccurredAt"] = DateTime.UtcNow.ToString("o");

            await sender.SendMessageAsync(message);
        }
    }
}
