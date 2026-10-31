namespace CreditCardApp.Modules.Payments.Domain;

public sealed class Payment
{
    public const int DeclineReasonMaxLength = 200;

    private Payment()
    {
        CardLast4Digits = string.Empty;
        CardBrand = string.Empty;
        Currency = string.Empty;
        Amount = null!;
        Merchant = null!;
    }

    private Payment(
        Guid userId,
        CardSnapshot card,
        PaymentAmount amount,
        Merchant merchant,
        PaymentStatus status,
        string? declineReason,
        DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CardId = card.CardId;
        CardLast4Digits = card.Last4Digits;
        CardBrand = card.Brand;
        Currency = card.Currency;
        Amount = amount;
        Merchant = merchant;
        Status = status;
        DeclineReason = declineReason;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid CardId { get; private set; }

    public string CardLast4Digits { get; private set; }

    public string CardBrand { get; private set; }

    public string Currency { get; private set; }

    public PaymentAmount Amount { get; private set; }

    public Merchant Merchant { get; private set; }

    public PaymentStatus Status { get; private set; }

    public string? DeclineReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Payment Approve(Guid userId, CardSnapshot card, PaymentAmount amount, Merchant merchant, DateTimeOffset now) =>
        new(userId, card, amount, merchant, PaymentStatus.Approved, null, now);

    public static Payment Decline(Guid userId, CardSnapshot card, PaymentAmount amount, Merchant merchant, string? reason, DateTimeOffset now)
    {
        var normalizedReason = string.IsNullOrWhiteSpace(reason) ? PaymentErrors.DefaultDeclineReason : reason.Trim();

        if (normalizedReason.Length > DeclineReasonMaxLength)
        {
            normalizedReason = normalizedReason[..DeclineReasonMaxLength];
        }

        return new Payment(userId, card, amount, merchant, PaymentStatus.Declined, normalizedReason, now);
    }
}
