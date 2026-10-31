using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application;

internal static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "Correo o contraseña incorrectos.");

    public static readonly Error EmailAlreadyRegistered =
        Error.Conflict("Auth.EmailAlreadyRegistered", "El correo ya se encuentra registrado.");

    public static readonly Error UserNotFound =
        Error.NotFound("Auth.UserNotFound", "El usuario no existe.");
}
