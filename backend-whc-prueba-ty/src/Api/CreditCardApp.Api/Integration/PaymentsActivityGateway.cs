using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Application.Ports.In;

namespace CreditCardApp.Api.Integration;

internal sealed class PaymentsActivityGateway(IPaymentActivityQuery paymentActivity) : ICardPaymentActivity
{
    public Task<bool> HasApprovedPaymentsSinceAsync(Guid ownerId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken) =>
        paymentActivity.HasApprovedPaymentsSinceAsync(ownerId, cardId, since, cancellationToken);
}
