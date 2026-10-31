namespace CreditCardApp.Api.Contracts.Payments;

public sealed record CreatePaymentRequest(Guid CardId, decimal Amount, string Merchant);
