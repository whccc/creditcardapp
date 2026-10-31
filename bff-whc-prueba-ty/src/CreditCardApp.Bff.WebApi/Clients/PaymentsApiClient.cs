using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Payments;

namespace CreditCardApp.Bff.WebApi.Clients;

public sealed class PaymentsApiClient(HttpClient httpClient) : CoreApiClient(httpClient)
{
    private const string BasePath = "api/v1/payments";

    public Task<DownstreamResult<PaymentDto>> ProcessAsync(CreatePaymentRequest request, CancellationToken cancellationToken) =>
        PostAsync<PaymentDto>(BasePath, request, cancellationToken);

    public Task<DownstreamResult<PagedResult<PaymentDto>>> GetHistoryAsync(
        Guid? cardId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = $"page={page}&pageSize={pageSize}{(cardId is { } id ? $"&cardId={id}" : string.Empty)}";

        return GetAsync<PagedResult<PaymentDto>>($"{BasePath}?{query}", cancellationToken);
    }
}
