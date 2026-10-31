using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.Ports.Out;

namespace CreditCardApp.Modules.Payments.Application.UseCases;

internal sealed class PaymentActivityQuery(IPaymentRepository payments) : IPaymentActivityQuery
{
    public Task<bool> HasApprovedPaymentsSinceAsync(Guid userId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken = default) =>
        payments.HasApprovedPaymentsSinceAsync(userId, cardId, since, cancellationToken);
}
