using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.Ports.Out;

public interface ICardRepository
{
    Task<Card?> GetAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Card>> ListAsync(Guid ownerId, CancellationToken cancellationToken);

    void Add(Card card);

    void Remove(Card card);

    Task<Result> SaveChangesAsync(CancellationToken cancellationToken);
}
