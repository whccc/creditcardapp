using CreditCardApp.Modules.Authentication.Domain;

namespace CreditCardApp.Modules.Authentication.Application.Contracts;

public sealed record UserDto(Guid Id, string Email, string FullName)
{
    internal static UserDto From(User user) => new(user.Id, user.Email.Value, user.FullName);
}
