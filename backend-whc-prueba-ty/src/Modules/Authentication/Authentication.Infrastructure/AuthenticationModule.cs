using CreditCardApp.Modules.Authentication.Application;
using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.Modules.Authentication.Infrastructure.Persistence;
using CreditCardApp.Modules.Authentication.Infrastructure.Security;
using CreditCardApp.SharedKernel.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditCardApp.Modules.Authentication.Infrastructure;

public static class AuthenticationModule
{
    public const string ConnectionStringName = "Authentication";

    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationApplication();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();

        services.AddScoped<IModuleInitializer, AuthenticationDatabaseInitializer>();

        return services;
    }
}
