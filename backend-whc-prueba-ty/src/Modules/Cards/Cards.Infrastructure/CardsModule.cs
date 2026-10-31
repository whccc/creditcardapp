using CreditCardApp.Modules.Cards.Application;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Infrastructure.Persistence;
using CreditCardApp.SharedKernel.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditCardApp.Modules.Cards.Infrastructure;

public static class CardsModule
{
    public const string ConnectionStringName = "Cards";

    public static IServiceCollection AddCardsModule<TCardPaymentActivity>(this IServiceCollection services, IConfiguration configuration)
        where TCardPaymentActivity : class, ICardPaymentActivity
    {
        services.AddCardsApplication();

        services.AddOptions<CardsOptions>()
            .Bind(configuration.GetSection(CardsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<CardsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.")));

        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardPaymentActivity, TCardPaymentActivity>();

        services.AddScoped<IModuleInitializer, CardsDatabaseInitializer>();

        return services;
    }
}
