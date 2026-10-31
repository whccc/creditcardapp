namespace CreditCardApp.Api.Contracts.Cards;

public sealed record CreateCardRequest(
    string CardNumber,
    string HolderName,
    string? Nickname,
    int ExpiryMonth,
    int ExpiryYear,
    decimal CreditLimit);

public sealed record UpdateCardRequest(string HolderName, string? Nickname, decimal CreditLimit);
