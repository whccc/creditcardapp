using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Common;
using CreditCardApp.Bff.WebApi.Contracts.Payments;

namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class PaymentMappings
{
    public static PaymentResponse ToResponse(this PaymentDto payment) => new(
        payment.Id,
        payment.CardId,
        payment.CardBrand.ToLowerInvariant(),
        payment.CardLast4Digits,
        $"{BrandNames.For(payment.CardBrand)} •••• {payment.CardLast4Digits}",
        payment.Merchant,
        payment.Amount,
        payment.Currency,
        payment.Status.ToLowerInvariant(),
        payment.DeclineReason,
        payment.CreatedAt);

    public static PagedResponse<PaymentResponse> ToResponse(this PagedResult<PaymentDto> page) => new(
        page.Items.Select(ToResponse).ToList(),
        page.Page,
        page.PageSize,
        page.TotalCount,
        page.TotalPages,
        page.Page > 1,
        page.Page < page.TotalPages);
}
