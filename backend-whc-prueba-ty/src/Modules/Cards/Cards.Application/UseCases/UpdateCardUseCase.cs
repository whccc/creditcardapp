using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class UpdateCardUseCase(ICardRepository cards, TimeProvider timeProvider) : IUpdateCardUseCase
{
    public async Task<Result<CardDto>> ExecuteAsync(UpdateCardCommand command, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(command.OwnerId, command.CardId, cancellationToken);
        if (card is null)
        {
            return CardErrors.NotFound;
        }

        var now = timeProvider.GetUtcNow();
        var update = card.UpdateDetails(command.HolderName, command.Nickname, command.CreditLimit, now);
        if (update.IsFailure)
        {
            return update.Error;
        }

        var saved = await cards.SaveChangesAsync(cancellationToken);

        return saved.IsSuccess ? CardDto.From(card, now) : saved.Error;
    }
}
