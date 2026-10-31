using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.Ports.In;

public interface IRegisterUserUseCase
{
    Task<Result<UserDto>> ExecuteAsync(RegisterUserCommand command, CancellationToken cancellationToken = default);
}
