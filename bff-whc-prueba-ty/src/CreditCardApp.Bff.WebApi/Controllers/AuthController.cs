using CreditCardApp.Bff.WebApi.Clients;
using CreditCardApp.Bff.WebApi.Contracts.Auth;
using CreditCardApp.Bff.WebApi.Extensions;
using CreditCardApp.Bff.WebApi.Http;
using CreditCardApp.Bff.WebApi.Mapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CreditCardApp.Bff.WebApi.Controllers;

[Route($"{RoutePrefix}/auth")]
[Tags("Auth")]
public sealed class AuthController(AuthApiClient authApi) : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(LoginRateLimitOptions.PolicyName)]
    [EndpointSummary("Autentica contra el core y abre la sesión en una cookie HttpOnly. El token nunca llega al navegador.")]
    [ProducesResponseType<SessionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SessionResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authApi.LoginAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return Relay(result);
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            SessionTicket.CreatePrincipal(result.Value.User),
            SessionTicket.CreateProperties(result.Value));

        return Ok(result.Value.ToSessionResponse());
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [EndpointSummary("Cierra la sesión y elimina la cookie.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }

    [HttpGet("session")]
    [EndpointSummary("Devuelve la sesión activa (usuario y expiración). 401 si no hay sesión.")]
    [ProducesResponseType<SessionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SessionResponse>> GetSession(CancellationToken cancellationToken)
    {
        var user = await authApi.GetCurrentUserAsync(cancellationToken);
        if (!user.IsSuccess)
        {
            return Relay(user);
        }

        var session = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var expiresAt = session.Properties?.ExpiresUtc ?? DateTimeOffset.UtcNow;

        return Ok(new SessionResponse(expiresAt, user.Value.ToResponse()));
    }
}
