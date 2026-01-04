using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Events.Domain.Users;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Domain.Users.Entities;
using FCG_Users.Domain.Users.Enums;
using Fgc.Domain.Usuario.ObjetosDeValor;
using System.Text.Json;

namespace FCG_Users.Consumer.Consumers
{    
    public class UsersTopicConsumer : IHostedService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UsersTopicConsumer> _logger;

        public UsersTopicConsumer(ServiceBusClient client, IConfiguration cfg, IServiceScopeFactory scopeFactory, ILogger<UsersTopicConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var topicName = cfg["ServiceBus:Topics:Users"];
            var subscriptionName = cfg["ServiceBus:Subscriptions:Users"];

            _processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 4,
                PrefetchCount = 20
            });

            _processor.ProcessMessageAsync += OnMessageAsync;
            _processor.ProcessErrorAsync += OnErrorAsync;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer iniciado para {Topic}/{Subscription}", _processor.EntityPath, "users-api-sub");
            await _processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer parado");
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        private async Task OnMessageAsync(ProcessMessageEventArgs args)
        {
            var subject = args.Message.Subject;
            var body = args.Message.Body.ToString();

            _logger.LogInformation("Mensagem recebida: Subject={Subject}, CorrelationId={CorrelationId}", subject, args.Message.CorrelationId);

            switch (subject)
            {
                case "UserCreated":
                    await HandleUserCreatedEvent(body);
                    break;
                case "UserDeleted":
                    await HandleUserDeletedEvent(body);
                    break;               
                default:
                    _logger.LogWarning("Evento desconhecido: {Subject}", subject);
                    break;
            }

            await args.CompleteMessageAsync(args.Message);
        }       

        private async Task HandleUserDeletedEvent(string body)
        {
            var evt = JsonSerializer.Deserialize<UserDeletedEvent>(body);

            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            var userId = Guid.Parse(evt!.AggregateId);

            var user = await repo.GetByIdAsync(userId);
            if (user is not null)
                await repo.DeleteAsync(user.Id);

        }

        private async Task HandleUserCreatedEvent(string body)
        {           
            var evt = JsonSerializer.Deserialize<UserCreatedEvent>(body);

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var user = Account.Create(evt!.Name, evt.Password, evt.Email, Enum.Parse<EProfileType>(evt.Profile), evt.Active);           

            if (!await repo.Exists(user.Email))
                await repo.CreateAsync(user);
         
        }

        private Task OnErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Erro no consumer: {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        }
    }
}
