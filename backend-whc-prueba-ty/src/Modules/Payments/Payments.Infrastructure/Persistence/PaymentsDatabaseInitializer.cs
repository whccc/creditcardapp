using CreditCardApp.SharedKernel.Modules;

namespace CreditCardApp.Modules.Payments.Infrastructure.Persistence;

internal sealed class PaymentsDatabaseInitializer(PaymentsDbContext dbContext) : IModuleInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken) =>
        dbContext.Database.EnsureCreatedAsync(cancellationToken);
}
