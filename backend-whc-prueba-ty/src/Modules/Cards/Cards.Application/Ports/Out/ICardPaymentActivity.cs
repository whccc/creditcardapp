namespace CreditCardApp.Modules.Cards.Application.Ports.Out;

public interface ICardPaymentActivity
{
    Task<bool> HasApprovedPaymentsSinceAsync(Guid ownerId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken);
}
