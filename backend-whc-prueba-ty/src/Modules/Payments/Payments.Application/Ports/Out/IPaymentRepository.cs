using CreditCardApp.Modules.Payments.Domain;

namespace CreditCardApp.Modules.Payments.Application.Ports.Out;

public interface IPaymentRepository
{
    void Add(Payment payment);

    Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPageAsync(
        Guid userId,
        Guid? cardId,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<bool> HasApprovedPaymentsSinceAsync(Guid userId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
