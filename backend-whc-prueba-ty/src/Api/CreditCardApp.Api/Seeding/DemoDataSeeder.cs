using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.SharedKernel.Results;
using Microsoft.Extensions.Options;

namespace CreditCardApp.Api.Seeding;

internal sealed class DemoDataSeeder(
    IRegisterUserUseCase registerUser,
    ICreateCardUseCase createCard,
    IChangeCardStatusUseCase changeCardStatus,
    IProcessPaymentUseCase processPayment,
    IOptions<DemoDataOptions> options,
    TimeProvider timeProvider,
    ILogger<DemoDataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.Enabled)
        {
            return;
        }

        var user = await registerUser.ExecuteAsync(
            new RegisterUserCommand(settings.Email, settings.FullName, settings.Password),
            cancellationToken);

        if (user.IsFailure)
        {
            if (user.Error.Type == ErrorType.Conflict)
            {
                logger.LogInformation("Demo data already present for {Email}; seeding skipped", settings.Email);
                return;
            }

            throw new InvalidOperationException($"Demo user could not be created: {user.Error.Message}");
        }

        var userId = user.Value.Id;
        var currentYear = timeProvider.GetUtcNow().Year;

        var visa = await CreateCardAsync(
            new CreateCardCommand(userId, "4111 1111 1111 1111", settings.FullName, "Tarjeta principal", 8, currentYear + 3, 5_000m),
            cancellationToken);
        var mastercard = await CreateCardAsync(
            new CreateCardCommand(userId, "5555 5555 5555 4444", settings.FullName, "Compras online", 11, currentYear + 2, 3_000m),
            cancellationToken);
        var amex = await CreateCardAsync(
            new CreateCardCommand(userId, "3782 822463 10005", settings.FullName, "Viajes", 3, currentYear + 4, 12_000m),
            cancellationToken);

        (Guid CardId, string Merchant, decimal Amount)[] payments =
        [
            (visa, "Supermercado Central", 154.90m),
            (visa, "Netflix", 15.99m),
            (visa, "Estación de Servicio", 62.40m),
            (mastercard, "Amazon", 249.99m),
            (mastercard, "Spotify", 10.99m),
            (amex, "LATAM Airlines", 1_890.00m),
            (mastercard, "Apple Store", 4_999.00m)
        ];

        foreach (var (cardId, merchant, amount) in payments)
        {
            await processPayment.ExecuteAsync(new ProcessPaymentCommand(userId, cardId, amount, merchant), cancellationToken);
        }

        await changeCardStatus.ExecuteAsync(new ChangeCardStatusCommand(userId, mastercard, Block: true), cancellationToken);

        logger.LogInformation("Demo data seeded for {Email}", settings.Email);
    }

    private async Task<Guid> CreateCardAsync(CreateCardCommand command, CancellationToken cancellationToken)
    {
        var card = await createCard.ExecuteAsync(command, cancellationToken);

        return card.IsSuccess
            ? card.Value.Id
            : throw new InvalidOperationException($"Demo card could not be created: {card.Error.Message}");
    }
}
