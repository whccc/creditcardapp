using CreditCardApp.Modules.Payments.Application;
using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Infrastructure.Persistence;
using CreditCardApp.SharedKernel.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditCardApp.Modules.Payments.Infrastructure;

public static class PaymentsModule
{
    public const string ConnectionStringName = "Payments";

    public static IServiceCollection AddPaymentsModule<TCardChargeGateway>(this IServiceCollection services, IConfiguration configuration)
        where TCardChargeGateway : class, ICardChargeGateway
    {
        services.AddPaymentsApplication();

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICardChargeGateway, TCardChargeGateway>();

        services.AddScoped<IModuleInitializer, PaymentsDatabaseInitializer>();

        return services;
    }
}
