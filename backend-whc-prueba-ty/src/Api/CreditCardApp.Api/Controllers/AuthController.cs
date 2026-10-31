using CreditCardApp.Api.Contracts.Auth;
using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Api.Controllers;

[Route($"{RoutePrefix}/auth")]
[Tags("Auth")]
public sealed class AuthController(ILoginUseCase login) : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Valida credenciales y emite un JWT.")]
    [ProducesResponseType<AuthenticationResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResult>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await login.ExecuteAsync(new LoginCommand(request.Email, request.Password), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }
}
