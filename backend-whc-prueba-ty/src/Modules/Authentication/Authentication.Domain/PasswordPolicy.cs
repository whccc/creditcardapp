using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Domain;

public static class PasswordPolicy
{
    public const int MinLength = 8;

    public static Result Validate(string? password)
    {
        var isStrong = password is { Length: >= MinLength }
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit);

        return isStrong ? Result.Success() : UserErrors.WeakPassword;
    }
}
