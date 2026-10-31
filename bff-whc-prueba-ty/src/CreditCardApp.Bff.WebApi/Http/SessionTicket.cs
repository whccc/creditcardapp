using System.Security.Claims;
using CreditCardApp.Bff.WebApi.Clients.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CreditCardApp.Bff.WebApi.Http;

internal static class SessionTicket
{
    public const string CookieName = "ccapp.session";
    public const string AccessTokenName = "access_token";

    public static ClaimsPrincipal CreatePrincipal(UserDto user) =>
        new(new ClaimsIdentity(
            [
                new Claim("sub", user.Id.ToString()),
                new Claim("name", user.FullName),
                new Claim("email", user.Email)
            ],
            CookieAuthenticationDefaults.AuthenticationScheme,
            "name",
            null));

    public static AuthenticationProperties CreateProperties(AuthenticationResult authentication)
    {
        var properties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = false,
            ExpiresUtc = authentication.ExpiresAt
        };

        properties.StoreTokens([new AuthenticationToken { Name = AccessTokenName, Value = authentication.AccessToken }]);

        return properties;
    }

    public static Task<string?> GetAccessTokenAsync(HttpContext httpContext) =>
        httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, AccessTokenName);
}
