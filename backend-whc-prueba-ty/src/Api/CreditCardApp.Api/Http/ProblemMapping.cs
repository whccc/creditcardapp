using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Api.Http;

internal static class ProblemMapping
{
    public static (int StatusCode, string Title) ToHttpStatus(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
        ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "No autorizado"),
        ErrorType.NotFound => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
        ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflicto"),
        ErrorType.BusinessRule => (StatusCodes.Status422UnprocessableEntity, "Operación no permitida"),
        _ => (StatusCodes.Status500InternalServerError, "Error inesperado")
    };
}
