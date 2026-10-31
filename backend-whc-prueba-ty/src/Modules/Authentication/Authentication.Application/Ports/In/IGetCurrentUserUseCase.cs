using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.Ports.In;

public interface IGetCurrentUserUseCase
{
    Task<Result<UserDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}
