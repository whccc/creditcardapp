using CreditCardApp.Modules.Authentication.Domain;

namespace CreditCardApp.Modules.Authentication.Application.Ports.Out;

public interface IAccessTokenIssuer
{
    AccessToken Issue(User user);
}

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);
