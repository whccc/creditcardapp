using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Domain;

public static class UserErrors
{
    public static readonly Error InvalidEmail =
        Error.Validation("User.InvalidEmail", "El correo electrónico no es válido.");

    public static readonly Error InvalidFullName =
        Error.Validation("User.InvalidFullName", $"El nombre debe tener entre {User.FullNameMinLength} y {User.FullNameMaxLength} caracteres.");

    public static readonly Error WeakPassword =
        Error.Validation("User.WeakPassword", $"La contraseña debe tener al menos {PasswordPolicy.MinLength} caracteres e incluir mayúsculas, minúsculas y números.");
}
