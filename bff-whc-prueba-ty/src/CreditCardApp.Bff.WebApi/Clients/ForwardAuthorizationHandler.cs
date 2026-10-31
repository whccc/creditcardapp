using System.Net.Http.Headers;
using CreditCardApp.Bff.WebApi.Http;

namespace CreditCardApp.Bff.WebApi.Clients;

internal sealed class ForwardAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is not null && request.Headers.Authorization is null)
        {
            var accessToken = await SessionTicket.GetAccessTokenAsync(httpContext);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
