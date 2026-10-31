using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.Modules.Authentication.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.UseCases;

internal sealed class RegisterUserUseCase(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRegisterUserUseCase
{
    public async Task<Result<UserDto>> ExecuteAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(command.Email);
        if (email.IsFailure)
        {
            return email.Error;
        }

        var passwordCheck = PasswordPolicy.Validate(command.Password);
        if (passwordCheck.IsFailure)
        {
            return passwordCheck.Error;
        }

        if (await users.ExistsAsync(email.Value, cancellationToken))
        {
            return AuthenticationErrors.EmailAlreadyRegistered;
        }

        var user = User.Create(email.Value, command.FullName, timeProvider.GetUtcNow());
        if (user.IsFailure)
        {
            return user.Error;
        }

        user.Value.SetPasswordHash(passwordHasher.Hash(user.Value, command.Password));

        users.Add(user.Value);
        await users.SaveChangesAsync(cancellationToken);

        return UserDto.From(user.Value);
    }
}
