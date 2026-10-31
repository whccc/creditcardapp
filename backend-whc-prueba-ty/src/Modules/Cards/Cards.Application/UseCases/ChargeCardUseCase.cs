using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class ChargeCardUseCase(ICardRepository cards, TimeProvider timeProvider) : IChargeCardUseCase
{
    public async Task<Result<CardChargeReceipt>> ExecuteAsync(ChargeCardCommand command, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(command.OwnerId, command.CardId, cancellationToken);
        if (card is null)
        {
            return CardErrors.NotFound;
        }

        var charge = card.Charge(command.Amount, timeProvider.GetUtcNow());
        if (charge.IsFailure)
        {
            return charge.Error;
        }

        var saved = await cards.SaveChangesAsync(cancellationToken);
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        return new CardChargeReceipt(card.Id, command.Amount, card.AvailableCredit, card.Currency);
    }
}
