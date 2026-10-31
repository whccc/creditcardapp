using System.Text.RegularExpressions;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Domain;

public sealed partial record Email
{
    public const int MaxLength = 254;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Result<Email> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(normalized) || normalized.Length > MaxLength || !EmailPattern().IsMatch(normalized))
        {
            return UserErrors.InvalidEmail;
        }

        return new Email(normalized);
    }

    public static Email FromPersistence(string value) => new(value);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();
}
