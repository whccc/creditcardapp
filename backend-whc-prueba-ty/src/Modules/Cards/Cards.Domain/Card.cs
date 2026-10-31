using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Domain;

public sealed class Card
{
    public const int HolderNameMinLength = 2;
    public const int HolderNameMaxLength = 60;
    public const int NicknameMaxLength = 40;
    public const decimal MaxCreditLimit = 1_000_000m;

    private Card()
    {
        HolderName = string.Empty;
        Last4Digits = string.Empty;
        Currency = string.Empty;
        Expiry = null!;
    }

    private Card(
        Guid ownerId,
        string holderName,
        string? nickname,
        CardNumber number,
        ExpiryDate expiry,
        decimal creditLimit,
        string currency,
        DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        HolderName = holderName;
        Nickname = nickname;
        Last4Digits = number.Last4Digits;
        Brand = number.Brand;
        Expiry = expiry;
        CreditLimit = creditLimit;
        AvailableCredit = creditLimit;
        Currency = currency;
        Status = CardStatus.Active;
        CreatedAt = now;
        UpdatedAt = now;
        Version = Guid.NewGuid();
    }

    public Guid Id { get; private set; }

    public Guid OwnerId { get; private set; }

    public string HolderName { get; private set; }

    public string? Nickname { get; private set; }

    public string Last4Digits { get; private set; }

    public CardBrand Brand { get; private set; }

    public ExpiryDate Expiry { get; private set; }

    public decimal CreditLimit { get; private set; }

    public decimal AvailableCredit { get; private set; }

    public decimal UsedCredit => CreditLimit - AvailableCredit;

    public string Currency { get; private set; }

    public CardStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid Version { get; private set; }

    public static Result<Card> Create(
        Guid ownerId,
        string? holderName,
        string? nickname,
        CardNumber number,
        ExpiryDate expiry,
        decimal creditLimit,
        string currency,
        DateTimeOffset now)
    {
        var details = ValidateDetails(holderName, nickname, creditLimit);
        if (details.IsFailure)
        {
            return details.Error;
        }

        if (expiry.IsExpiredAt(now))
        {
            return CardErrors.ExpiryInThePast;
        }

        var (normalizedHolder, normalizedNickname) = details.Value;

        return new Card(ownerId, normalizedHolder, normalizedNickname, number, expiry, creditLimit, currency, now);
    }

    public bool IsExpiredAt(DateTimeOffset now) => Expiry.IsExpiredAt(now);

    public Result UpdateDetails(string? holderName, string? nickname, decimal creditLimit, DateTimeOffset now)
    {
        var details = ValidateDetails(holderName, nickname, creditLimit);
        if (details.IsFailure)
        {
            return details.Error;
        }

        if (creditLimit < UsedCredit)
        {
            return CardErrors.CreditLimitBelowUsage;
        }

        var usedCredit = UsedCredit;
        (HolderName, Nickname) = details.Value;
        CreditLimit = creditLimit;
        AvailableCredit = creditLimit - usedCredit;
        Touch(now);

        return Result.Success();
    }

    public void Block(DateTimeOffset now)
    {
        if (Status == CardStatus.Blocked)
        {
            return;
        }

        Status = CardStatus.Blocked;
        Touch(now);
    }

    public void Unblock(DateTimeOffset now)
    {
        if (Status == CardStatus.Active)
        {
            return;
        }

        Status = CardStatus.Active;
        Touch(now);
    }

    public Result Charge(decimal amount, DateTimeOffset now)
    {
        if (!IsPositiveMoney(amount))
        {
            return CardErrors.InvalidChargeAmount;
        }

        if (Status == CardStatus.Blocked)
        {
            return CardErrors.Blocked;
        }

        if (IsExpiredAt(now))
        {
            return CardErrors.Expired;
        }

        if (amount > AvailableCredit)
        {
            return CardErrors.InsufficientCredit;
        }

        AvailableCredit -= amount;
        Touch(now);

        return Result.Success();
    }

    private static Result<(string HolderName, string? Nickname)> ValidateDetails(string? holderName, string? nickname, decimal creditLimit)
    {
        var normalizedHolder = holderName?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalizedHolder.Length is < HolderNameMinLength or > HolderNameMaxLength)
        {
            return CardErrors.InvalidHolderName;
        }

        var normalizedNickname = string.IsNullOrWhiteSpace(nickname) ? null : nickname.Trim();
        if (normalizedNickname is { Length: > NicknameMaxLength })
        {
            return CardErrors.InvalidNickname;
        }

        if (!IsPositiveMoney(creditLimit) || creditLimit > MaxCreditLimit)
        {
            return CardErrors.InvalidCreditLimit;
        }

        return (normalizedHolder, normalizedNickname);
    }

    private static bool IsPositiveMoney(decimal amount) => amount > 0 && decimal.Round(amount, 2) == amount;

    private void Touch(DateTimeOffset now)
    {
        UpdatedAt = now;
        Version = Guid.NewGuid();
    }
}
