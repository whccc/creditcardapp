using CreditCardApp.SharedKernel.Modules;

namespace CreditCardApp.Modules.Authentication.Infrastructure.Persistence;

internal sealed class AuthenticationDatabaseInitializer(AuthenticationDbContext dbContext) : IModuleInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken) =>
        dbContext.Database.EnsureCreatedAsync(cancellationToken);
}
