using CreditCardApp.Modules.Cards.Domain;

namespace CreditCardApp.Modules.Cards.Application.Contracts;

public sealed record CardDto(
    Guid Id,
    string HolderName,
    string? Nickname,
    string Last4Digits,
    string Brand,
    int ExpiryMonth,
    int ExpiryYear,
    decimal CreditLimit,
    decimal AvailableCredit,
    decimal UsedCredit,
    string Currency,
    string Status,
    bool IsExpired,
    DateTimeOffset CreatedAt)
{
    internal static CardDto From(Card card, DateTimeOffset now) => new(
        card.Id,
        card.HolderName,
        card.Nickname,
        card.Last4Digits,
        card.Brand.ToString(),
        card.Expiry.Month,
        card.Expiry.Year,
        card.CreditLimit,
        card.AvailableCredit,
        card.UsedCredit,
        card.Currency,
        card.Status.ToString(),
        card.IsExpiredAt(now),
        card.CreatedAt);
}
