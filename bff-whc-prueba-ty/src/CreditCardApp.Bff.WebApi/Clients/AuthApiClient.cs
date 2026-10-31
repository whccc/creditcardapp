using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Auth;

namespace CreditCardApp.Bff.WebApi.Clients;

public sealed class AuthApiClient(HttpClient httpClient) : CoreApiClient(httpClient)
{
    public Task<DownstreamResult<AuthenticationResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken) =>
        PostAsync<AuthenticationResult>("api/v1/auth/login", request, cancellationToken);

    public Task<DownstreamResult<UserDto>> GetCurrentUserAsync(CancellationToken cancellationToken) =>
        GetAsync<UserDto>("api/v1/users/me", cancellationToken);
}
