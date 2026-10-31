using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Http;

internal sealed class CsrfHeaderMiddleware(RequestDelegate next, IProblemDetailsService problemDetailsService)
{
    public const string HeaderName = "X-CSRF";
    public const string HeaderValue = "1";

    public async Task InvokeAsync(HttpContext context)
    {
        var isProtectedRequest = context.Request.Path.StartsWithSegments("/api") && !HttpMethods.IsOptions(context.Request.Method);

        if (isProtectedRequest && context.Request.Headers[HeaderName] != HeaderValue)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Solicitud rechazada",
                    Detail = $"Falta el header {HeaderName}.",
                    Extensions = { ["code"] = "Bff.CsrfHeaderMissing" }
                }
            });
            return;
        }

        await next(context);
    }
}
