using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreditCardApp.Modules.Cards.Application;

public static class CardsApplication
{
    public static IServiceCollection AddCardsApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IGetCardsUseCase, GetCardsUseCase>();
        services.AddScoped<IGetCardByIdUseCase, GetCardByIdUseCase>();
        services.AddScoped<ICreateCardUseCase, CreateCardUseCase>();
        services.AddScoped<IUpdateCardUseCase, UpdateCardUseCase>();
        services.AddScoped<IChangeCardStatusUseCase, ChangeCardStatusUseCase>();
        services.AddScoped<IDeleteCardUseCase, DeleteCardUseCase>();
        services.AddScoped<IChargeCardUseCase, ChargeCardUseCase>();

        return services;
    }
}
