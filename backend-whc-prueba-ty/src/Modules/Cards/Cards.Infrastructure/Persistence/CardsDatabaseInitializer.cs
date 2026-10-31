using CreditCardApp.SharedKernel.Modules;

namespace CreditCardApp.Modules.Cards.Infrastructure.Persistence;

internal sealed class CardsDatabaseInitializer(CardsDbContext dbContext) : IModuleInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken) =>
        dbContext.Database.EnsureCreatedAsync(cancellationToken);
}
