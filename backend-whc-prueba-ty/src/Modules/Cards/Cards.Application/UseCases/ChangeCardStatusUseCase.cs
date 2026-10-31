using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class ChangeCardStatusUseCase(ICardRepository cards, TimeProvider timeProvider) : IChangeCardStatusUseCase
{
    public async Task<Result<CardDto>> ExecuteAsync(ChangeCardStatusCommand command, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(command.OwnerId, command.CardId, cancellationToken);
        if (card is null)
        {
            return CardErrors.NotFound;
        }

        var now = timeProvider.GetUtcNow();

        if (command.Block)
        {
            card.Block(now);
        }
        else
        {
            card.Unblock(now);
        }

        var saved = await cards.SaveChangesAsync(cancellationToken);

        return saved.IsSuccess ? CardDto.From(card, now) : saved.Error;
    }
}
