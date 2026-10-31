using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreditCardApp.Modules.Payments.Application;

public static class PaymentsApplication
{
    public static IServiceCollection AddPaymentsApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IProcessPaymentUseCase, ProcessPaymentUseCase>();
        services.AddScoped<IGetPaymentHistoryUseCase, GetPaymentHistoryUseCase>();

        return services;
    }
}
