using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CreditCardApp.Api.Http;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(subject, out var userId)
            ? userId
            : throw new InvalidOperationException("The authenticated principal does not contain a valid subject claim.");
    }
}
