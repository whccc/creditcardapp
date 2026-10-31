using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;
using Microsoft.Extensions.Options;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class CreateCardUseCase(
    ICardRepository cards,
    IOptions<CardsOptions> options,
    TimeProvider timeProvider) : ICreateCardUseCase
{
    public async Task<Result<CardDto>> ExecuteAsync(CreateCardCommand command, CancellationToken cancellationToken = default)
    {
        var number = CardNumber.Create(command.CardNumber);
        if (number.IsFailure)
        {
            return number.Error;
        }

        var expiry = ExpiryDate.Create(command.ExpiryMonth, command.ExpiryYear);
        if (expiry.IsFailure)
        {
            return expiry.Error;
        }

        var now = timeProvider.GetUtcNow();
        var card = Card.Create(
            command.OwnerId,
            command.HolderName,
            command.Nickname,
            number.Value,
            expiry.Value,
            command.CreditLimit,
            options.Value.Currency,
            now);

        if (card.IsFailure)
        {
            return card.Error;
        }

        cards.Add(card.Value);

        var saved = await cards.SaveChangesAsync(cancellationToken);

        return saved.IsSuccess ? CardDto.From(card.Value, now) : saved.Error;
    }
}
