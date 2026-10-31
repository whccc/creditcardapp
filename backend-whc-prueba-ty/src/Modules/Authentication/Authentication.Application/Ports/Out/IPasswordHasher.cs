using CreditCardApp.Modules.Authentication.Domain;

namespace CreditCardApp.Modules.Authentication.Application.Ports.Out;

public interface IPasswordHasher
{
    string Hash(User user, string password);

    bool Verify(User user, string password);
}
