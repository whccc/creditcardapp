namespace CreditCardApp.Bff.WebApi.Contracts.Payments;

public sealed record CreatePaymentRequest(Guid CardId, decimal Amount, string Merchant);

public sealed record PaymentResponse(
    Guid Id,
    Guid CardId,
    string CardBrand,
    string CardLast4Digits,
    string CardLabel,
    string Merchant,
    decimal Amount,
    string Currency,
    string Status,
    string? DeclineReason,
    DateTimeOffset CreatedAt);
