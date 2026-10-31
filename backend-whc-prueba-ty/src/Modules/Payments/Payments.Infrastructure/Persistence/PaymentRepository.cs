using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Domain;
using Microsoft.EntityFrameworkCore;

namespace CreditCardApp.Modules.Payments.Infrastructure.Persistence;

internal sealed class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public void Add(Payment payment) => dbContext.Payments.Add(payment);

    public async Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPageAsync(
        Guid userId,
        Guid? cardId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Payments.AsNoTracking().Where(payment => payment.UserId == userId);

        if (cardId is { } filterCardId)
        {
            query = query.Where(payment => payment.CardId == filterCardId);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(payment => payment.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> HasApprovedPaymentsSinceAsync(Guid userId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken) =>
        dbContext.Payments.AnyAsync(
            payment => payment.UserId == userId
                && payment.CardId == cardId
                && payment.Status == PaymentStatus.Approved
                && payment.CreatedAt >= since,
            cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
