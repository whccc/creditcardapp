using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class DeleteCardUseCase(ICardRepository cards) : IDeleteCardUseCase
{
    public async Task<Result> ExecuteAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(ownerId, cardId, cancellationToken);
        if (card is null)
        {
            return CardErrors.NotFound;
        }

        cards.Remove(card);

        return await cards.SaveChangesAsync(cancellationToken);
    }
}
