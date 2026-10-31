using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Domain;

public sealed class User
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 100;

    private User()
    {
        Email = null!;
        FullName = string.Empty;
        PasswordHash = string.Empty;
    }

    private User(Guid id, Email email, string fullName, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        PasswordHash = string.Empty;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Email Email { get; private set; }

    public string FullName { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<User> Create(Email email, string? fullName, DateTimeOffset now)
    {
        var normalizedName = fullName?.Trim() ?? string.Empty;

        if (normalizedName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return UserErrors.InvalidFullName;
        }

        return new User(Guid.NewGuid(), email, normalizedName, now);
    }

    public void SetPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }
}
