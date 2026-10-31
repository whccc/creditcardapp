using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Cards;

namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class CardMappings
{
    public static CardResponse ToResponse(this CardDto card) => new(
        card.Id,
        card.Brand.ToLowerInvariant(),
        BrandNames.For(card.Brand),
        MaskNumber(card.Brand, card.Last4Digits),
        card.Last4Digits,
        card.HolderName,
        card.Nickname,
        $"{card.ExpiryMonth:00}/{card.ExpiryYear % 100:00}",
        card.ExpiryMonth,
        card.ExpiryYear,
        card.IsExpired,
        card.Status.ToLowerInvariant(),
        card.CreditLimit,
        card.AvailableCredit,
        card.UsedCredit,
        Percentage.Of(card.UsedCredit, card.CreditLimit),
        card.Currency);

    public static IReadOnlyList<CardResponse> ToResponses(this IEnumerable<CardDto> cards) =>
        cards.Select(ToResponse).ToList();

    private static string MaskNumber(string brand, string last4Digits) =>
        brand == BrandNames.Amex
            ? $"•••• •••••• •{last4Digits}"
            : $"•••• •••• •••• {last4Digits}";
}
