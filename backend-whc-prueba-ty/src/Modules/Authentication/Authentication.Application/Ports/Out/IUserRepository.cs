using CreditCardApp.Modules.Authentication.Domain;

namespace CreditCardApp.Modules.Authentication.Application.Ports.Out;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Email email, CancellationToken cancellationToken);

    void Add(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
