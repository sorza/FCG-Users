using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Services;
using FCG_Users.Application.Users.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Users.Application.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IValidator<AccountRequest>, AccountRequestValidator>();
            services.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();

            return services;
        }
    }
}
