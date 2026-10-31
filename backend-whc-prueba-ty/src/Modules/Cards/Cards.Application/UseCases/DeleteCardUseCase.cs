using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.UseCases;

internal sealed class DeleteCardUseCase(
    ICardRepository cards,
    ICardPaymentActivity paymentActivity,
    TimeProvider timeProvider) : IDeleteCardUseCase
{
    public async Task<Result> ExecuteAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken = default)
    {
        var card = await cards.GetAsync(ownerId, cardId, cancellationToken);
        if (card is null)
        {
            return CardErrors.NotFound;
        }

        var since = CardDeletionPolicy.RecentPaymentsSince(timeProvider.GetUtcNow());
        if (await paymentActivity.HasApprovedPaymentsSinceAsync(ownerId, cardId, since, cancellationToken))
        {
            return CardErrors.HasRecentPayments;
        }

        cards.Remove(card);

        return await cards.SaveChangesAsync(cancellationToken);
    }
}
