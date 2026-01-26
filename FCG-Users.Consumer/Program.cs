using Azure.Messaging.ServiceBus;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Consumer.Consumers;
using FCG_Users.Infrastructure.Shared.Context;
using FCG_Users.Infrastructure.Users.Repositorories;
using Microsoft.EntityFrameworkCore;

namespace FCG_Users.Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(options =>
                    {
                        options.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"];
                    });

                    services.AddDbContext<UsersDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    var connectionString = context.Configuration["ServiceBus:ConnectionString"];
                    services.AddSingleton(new ServiceBusClient(connectionString));

                    services.AddScoped<IAccountRepository, AccountRepository>();
                    services.AddHostedService<UsersTopicConsumer>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
