using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Infrastructure.Users.Repositorories;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Users.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
