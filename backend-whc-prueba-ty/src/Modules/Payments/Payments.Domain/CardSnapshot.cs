namespace CreditCardApp.Modules.Payments.Domain;

public sealed record CardSnapshot(Guid CardId, string Last4Digits, string Brand, string Currency);
