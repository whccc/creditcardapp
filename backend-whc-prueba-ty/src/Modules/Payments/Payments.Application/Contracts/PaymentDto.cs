using CreditCardApp.Modules.Payments.Domain;

namespace CreditCardApp.Modules.Payments.Application.Contracts;

public sealed record PaymentDto(
    Guid Id,
    Guid CardId,
    string CardLast4Digits,
    string CardBrand,
    decimal Amount,
    string Currency,
    string Merchant,
    string Status,
    string? DeclineReason,
    DateTimeOffset CreatedAt)
{
    public bool IsApproved => Status == nameof(PaymentStatus.Approved);

    internal static PaymentDto From(Payment payment) => new(
        payment.Id,
        payment.CardId,
        payment.CardLast4Digits,
        payment.CardBrand,
        payment.Amount.Value,
        payment.Currency,
        payment.Merchant.Name,
        payment.Status.ToString(),
        payment.DeclineReason,
        payment.CreatedAt);
}
