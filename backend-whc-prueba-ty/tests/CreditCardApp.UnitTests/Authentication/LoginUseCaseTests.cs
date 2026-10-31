using CreditCardApp.Modules.Authentication.Application;
using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.Modules.Authentication.Application.UseCases;
using CreditCardApp.Modules.Authentication.Domain;
using CreditCardApp.Modules.Authentication.Infrastructure.Security;

namespace CreditCardApp.UnitTests.Authentication;

public sealed class LoginUseCaseTests
{
    private const string Password = "Secret123";

    private readonly InMemoryUserRepository _users = new();
    private readonly IdentityPasswordHasher _hasher = new();
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        var user = User.Create(Email.Create("jane@example.com").Value, "Jane Doe", DateTimeOffset.UtcNow).Value;
        user.SetPasswordHash(_hasher.Hash(user, Password));
        _users.Add(user);

        _useCase = new LoginUseCase(_users, _hasher, new FakeTokenIssuer());
    }

    [Fact]
    public async Task ValidCredentials_IssueToken()
    {
        var result = await _useCase.ExecuteAsync(new LoginCommand(" JANE@example.com ", Password));

        Assert.True(result.IsSuccess);
        Assert.Equal("token", result.Value.AccessToken);
        Assert.Equal("jane@example.com", result.Value.User.Email);
    }

    [Theory]
    [InlineData("jane@example.com", "wrong-password")]
    [InlineData("unknown@example.com", Password)]
    [InlineData("not-an-email", Password)]
    [InlineData("jane@example.com", "")]
    public async Task InvalidCredentials_ReturnTheSameGenericError(string email, string password)
    {
        var result = await _useCase.ExecuteAsync(new LoginCommand(email, password));

        Assert.Equal(AuthenticationErrors.InvalidCredentials, result.Error);
    }

    private sealed class FakeTokenIssuer : IAccessTokenIssuer
    {
        public AccessToken Issue(User user) => new("token", DateTimeOffset.UtcNow.AddHours(1));
    }

    private sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_users.FirstOrDefault(user => user.Id == id));

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken) =>
            Task.FromResult(_users.FirstOrDefault(user => user.Email == email));

        public Task<bool> ExistsAsync(Email email, CancellationToken cancellationToken) =>
            Task.FromResult(_users.Any(user => user.Email == email));

        public void Add(User user) => _users.Add(user);

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
