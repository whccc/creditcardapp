namespace CreditCardApp.Bff.WebApi.Clients.Contracts;

public sealed record UserDto(Guid Id, string Email, string FullName);

public sealed record AuthenticationResult(string AccessToken, DateTimeOffset ExpiresAt, UserDto User);

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
    DateTimeOffset CreatedAt);

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
    DateTimeOffset CreatedAt);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
