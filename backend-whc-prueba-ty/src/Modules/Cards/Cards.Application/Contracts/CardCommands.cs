namespace CreditCardApp.Modules.Cards.Application.Contracts;

public sealed record CreateCardCommand(
    Guid OwnerId,
    string CardNumber,
    string HolderName,
    string? Nickname,
    int ExpiryMonth,
    int ExpiryYear,
    decimal CreditLimit);

public sealed record UpdateCardCommand(
    Guid OwnerId,
    Guid CardId,
    string HolderName,
    string? Nickname,
    decimal CreditLimit);

public sealed record ChangeCardStatusCommand(Guid OwnerId, Guid CardId, bool Block);

public sealed record ChargeCardCommand(Guid OwnerId, Guid CardId, decimal Amount);

public sealed record CardChargeReceipt(Guid CardId, decimal Amount, decimal AvailableCredit, string Currency);
