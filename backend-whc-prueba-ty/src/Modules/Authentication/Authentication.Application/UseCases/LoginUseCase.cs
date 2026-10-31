using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.Modules.Authentication.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.UseCases;

internal sealed class LoginUseCase(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IAccessTokenIssuer tokenIssuer) : ILoginUseCase
{
    public async Task<Result<AuthenticationResult>> ExecuteAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(command.Email);

        if (email.IsFailure || string.IsNullOrEmpty(command.Password))
        {
            return AuthenticationErrors.InvalidCredentials;
        }

        var user = await users.GetByEmailAsync(email.Value, cancellationToken);

        if (user is null || !passwordHasher.Verify(user, command.Password))
        {
            return AuthenticationErrors.InvalidCredentials;
        }

        var token = tokenIssuer.Issue(user);

        return new AuthenticationResult(token.Value, token.ExpiresAt, UserDto.From(user));
    }
}
