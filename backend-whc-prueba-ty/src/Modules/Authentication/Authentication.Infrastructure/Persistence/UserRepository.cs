using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.Modules.Authentication.Domain;
using Microsoft.EntityFrameworkCore;

namespace CreditCardApp.Modules.Authentication.Infrastructure.Persistence;

internal sealed class UserRepository(AuthenticationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<bool> ExistsAsync(Email email, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);

    public void Add(User user) => dbContext.Users.Add(user);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
