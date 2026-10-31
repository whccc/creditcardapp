using CreditCardApp.Modules.Authentication.Domain;
using Microsoft.AspNetCore.Identity;
using IPasswordHasherPort = CreditCardApp.Modules.Authentication.Application.Ports.Out.IPasswordHasher;

namespace CreditCardApp.Modules.Authentication.Infrastructure.Security;

internal sealed class IdentityPasswordHasher : IPasswordHasherPort
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(User user, string password) => _hasher.HashPassword(user, password);

    public bool Verify(User user, string password) =>
        _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
