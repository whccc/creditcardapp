using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class GetCardsUseCase(ICardRepository cards, TimeProvider timeProvider) : IGetCardsUseCase
{
    public async Task<IReadOnlyList<CardDto>> ExecuteAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();
        var ownedCards = await cards.ListAsync(ownerId, cancellationToken);

        return ownedCards.Select(card => CardDto.From(card, now)).ToList();
    }
}

internal sealed class GetCardByIdUseCase(ICardRepository cards, TimeProvider timeProvider) : IGetCardByIdUseCase
{
    public async Task<Result<CardDto>> ExecuteAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(ownerId, cardId, cancellationToken);

        return card is null ? CardErrors.NotFound : CardDto.From(card, timeProvider.GetUtcNow());
    }
}
