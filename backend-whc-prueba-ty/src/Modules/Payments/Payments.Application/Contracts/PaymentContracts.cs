namespace CreditCardApp.Modules.Payments.Application.Contracts;

public sealed record ProcessPaymentCommand(Guid UserId, Guid CardId, decimal Amount, string Merchant);

public sealed record PaymentHistoryQuery(Guid UserId, Guid? CardId, int Page, int PageSize)
{
    public const int MaxPageSize = 50;
}

public sealed record CardChargeRequest(Guid UserId, Guid CardId, decimal Amount);

public sealed record CardChargeOutcome(
    bool IsApproved,
    string CardLast4Digits,
    string CardBrand,
    string Currency,
    string? DeclineReason)
{
    public static CardChargeOutcome Approved(string cardLast4Digits, string cardBrand, string currency) =>
        new(true, cardLast4Digits, cardBrand, currency, null);

    public static CardChargeOutcome Declined(string cardLast4Digits, string cardBrand, string currency, string reason) =>
        new(false, cardLast4Digits, cardBrand, currency, reason);
}
