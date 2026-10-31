using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Authentication.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreditCardApp.Modules.Authentication.Application;

public static class AuthenticationApplication
{
    public static IServiceCollection AddAuthenticationApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IGetCurrentUserUseCase, GetCurrentUserUseCase>();

        return services;
    }
}
