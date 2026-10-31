using CreditCardApp.Bff.WebApi.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected const string RoutePrefix = "api/v1";

    protected ActionResult Relay(DownstreamResult result) =>
        new ObjectResult(result.Problem) { StatusCode = result.Problem.Status ?? StatusCodes.Status502BadGateway };
}
