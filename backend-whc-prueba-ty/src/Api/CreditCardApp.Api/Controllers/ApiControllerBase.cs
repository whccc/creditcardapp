using CreditCardApp.Api.Http;
using CreditCardApp.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Api.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected const string RoutePrefix = "api/v1";

    protected Guid CurrentUserId => User.GetUserId();

    protected ActionResult ErrorResult(Error error)
    {
        var (statusCode, title) = error.Type.ToHttpStatus();
        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode, title, detail: error.Message);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
