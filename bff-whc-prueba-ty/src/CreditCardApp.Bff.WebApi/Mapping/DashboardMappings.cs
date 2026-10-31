using CreditCardApp.Bff.WebApi.Contracts.Cards;
using CreditCardApp.Bff.WebApi.Contracts.Dashboard;

namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class DashboardMappings
{
    private const string ActiveStatus = "active";
    private const string DefaultCurrency = "USD";

    public static CreditSummaryResponse ToSummary(this IReadOnlyList<CardResponse> cards)
    {
        var totalLimit = cards.Sum(card => card.CreditLimit);
        var totalUsed = cards.Sum(card => card.UsedCredit);
        var activeCards = cards.Count(card => card.Status == ActiveStatus);

        return new CreditSummaryResponse(
            totalLimit,
            cards.Sum(card => card.AvailableCredit),
            totalUsed,
            Percentage.Of(totalUsed, totalLimit),
            cards.Count,
            activeCards,
            cards.Count - activeCards,
            cards.FirstOrDefault()?.Currency ?? DefaultCurrency);
    }
}
