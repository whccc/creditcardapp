using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Cards;

namespace CreditCardApp.Bff.WebApi.Clients;

public sealed class CardsApiClient(HttpClient httpClient) : CoreApiClient(httpClient)
{
    private const string BasePath = "api/v1/cards";

    public Task<DownstreamResult<IReadOnlyList<CardDto>>> GetAllAsync(CancellationToken cancellationToken) =>
        GetAsync<IReadOnlyList<CardDto>>(BasePath, cancellationToken);

    public Task<DownstreamResult<CardDto>> GetByIdAsync(Guid cardId, CancellationToken cancellationToken) =>
        GetAsync<CardDto>($"{BasePath}/{cardId}", cancellationToken);

    public Task<DownstreamResult<CardDto>> CreateAsync(CreateCardRequest request, CancellationToken cancellationToken) =>
        PostAsync<CardDto>(BasePath, request, cancellationToken);

    public Task<DownstreamResult<CardDto>> UpdateAsync(Guid cardId, UpdateCardRequest request, CancellationToken cancellationToken) =>
        PutAsync<CardDto>($"{BasePath}/{cardId}", request, cancellationToken);

    public Task<DownstreamResult<CardDto>> BlockAsync(Guid cardId, CancellationToken cancellationToken) =>
        PostAsync<CardDto>($"{BasePath}/{cardId}/block", null, cancellationToken);

    public Task<DownstreamResult<CardDto>> UnblockAsync(Guid cardId, CancellationToken cancellationToken) =>
        PostAsync<CardDto>($"{BasePath}/{cardId}/unblock", null, cancellationToken);

    public Task<DownstreamResult> DeleteAsync(Guid cardId, CancellationToken cancellationToken) =>
        DeleteAsync($"{BasePath}/{cardId}", cancellationToken);
}
