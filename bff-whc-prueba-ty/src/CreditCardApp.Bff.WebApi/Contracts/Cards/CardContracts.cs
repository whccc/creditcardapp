namespace CreditCardApp.Bff.WebApi.Contracts.Cards;

public sealed record CreateCardRequest(
    string CardNumber,
    string HolderName,
    string? Nickname,
    int ExpiryMonth,
    int ExpiryYear,
    decimal CreditLimit);

public sealed record UpdateCardRequest(string HolderName, string? Nickname, decimal CreditLimit);

public sealed record CardResponse(
    Guid Id,
    string Brand,
    string BrandName,
    string MaskedNumber,
    string Last4Digits,
    string HolderName,
    string? Nickname,
    string Expiry,
    int ExpiryMonth,
    int ExpiryYear,
    bool IsExpired,
    string Status,
    decimal CreditLimit,
    decimal AvailableCredit,
    decimal UsedCredit,
    decimal UtilizationPercentage,
    string Currency);
