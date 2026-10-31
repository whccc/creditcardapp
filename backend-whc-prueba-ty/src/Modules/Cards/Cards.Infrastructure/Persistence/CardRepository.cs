using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace CreditCardApp.Modules.Cards.Infrastructure.Persistence;

internal sealed class CardRepository(CardsDbContext dbContext) : ICardRepository
{
    public Task<Card?> GetAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken) =>
        dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId && card.OwnerId == ownerId, cancellationToken);

    public async Task<IReadOnlyList<Card>> ListAsync(Guid ownerId, CancellationToken cancellationToken) =>
        await dbContext.Cards
            .AsNoTracking()
            .Where(card => card.OwnerId == ownerId)
            .OrderBy(card => card.CreatedAt)
            .ToListAsync(cancellationToken);

    public void Add(Card card) => dbContext.Cards.Add(card);

    public void Remove(Card card) => dbContext.Cards.Remove(card);

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return CardErrors.ConcurrentUpdate;
        }
    }
}
