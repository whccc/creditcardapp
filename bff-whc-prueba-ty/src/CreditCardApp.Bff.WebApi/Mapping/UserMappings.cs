using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Contracts.Auth;

namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class UserMappings
{
    public static SessionUserResponse ToResponse(this UserDto user)
    {
        var nameParts = user.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var initials = string.Concat(nameParts.Take(2).Select(part => char.ToUpperInvariant(part[0])));

        return new SessionUserResponse(
            user.Id,
            user.FullName,
            nameParts.FirstOrDefault() ?? user.FullName,
            initials,
            user.Email);
    }

    public static SessionResponse ToSessionResponse(this AuthenticationResult result) =>
        new(result.ExpiresAt, result.User.ToResponse());
}
