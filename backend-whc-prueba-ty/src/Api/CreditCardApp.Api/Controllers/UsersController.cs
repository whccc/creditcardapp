using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Api.Controllers;

[Route($"{RoutePrefix}/users")]
[Tags("Users")]
public sealed class UsersController(IGetCurrentUserUseCase getCurrentUser) : ApiControllerBase
{
    [HttpGet("me")]
    [EndpointSummary("Devuelve el usuario dueño del token.")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await getCurrentUser.ExecuteAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }
}
