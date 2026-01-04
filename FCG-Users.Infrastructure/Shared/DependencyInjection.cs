using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Events.Store;
using FCG.Shared.Contracts.Interfaces;
using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Infrastructure.Shared.Context;
using FCG_Users.Infrastructure.Shared.Services;
using FCG_Users.Infrastructure.Users.Events;
using FCG_Users.Infrastructure.Users.Repositorories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FCG_Users.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var connectionString = configuration["ServiceBus:ConnectionString"];
            var topic = configuration["ServiceBus:Topics:Users"];

            services.AddSingleton(new ServiceBusClient(connectionString));

            services.AddScoped<IEventPublisher>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                return new ServiceBusEventPublisher(client, topic!);
            });

            var mongoString = configuration["MongoSettings:ConnectionString"];
            var mongoDb = configuration["MongoSettings:Database"];
            var mongoCollection = configuration["MongoSettings:Collection"];

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoString));

            services.AddScoped<IEventStore>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return new MongoEventStore(client, mongoDb!, mongoCollection!);
            });

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
