using CreditCardApp.Bff.WebApi.Contracts.Auth;
using CreditCardApp.Bff.WebApi.Contracts.Cards;

namespace CreditCardApp.Bff.WebApi.Contracts.Dashboard;

public sealed record CreditSummaryResponse(
    decimal TotalCreditLimit,
    decimal TotalAvailableCredit,
    decimal TotalUsedCredit,
    decimal UtilizationPercentage,
    int TotalCards,
    int ActiveCards,
    int BlockedCards,
    string Currency);

public sealed record DashboardResponse(
    SessionUserResponse User,
    CreditSummaryResponse Summary,
    IReadOnlyList<CardResponse> Cards);
