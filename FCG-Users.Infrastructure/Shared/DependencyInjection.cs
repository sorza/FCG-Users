using Azure.Messaging.ServiceBus;
using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Infrastructure.Shared.Context;
using FCG_Users.Infrastructure.Shared.Services;
using FCG_Users.Infrastructure.Users.Events;
using FCG_Users.Infrastructure.Users.Repositorories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Users.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var connectionString = configuration["ServiceBus:ConnectionString"];
            var queueName = configuration["ServiceBus:Queues:UsersEvents"];

            services.AddSingleton(new ServiceBusClient(connectionString));

            services.AddScoped<IEventPublisher>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                return new ServiceBusEventPublisher(client, queueName!);
            });

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
