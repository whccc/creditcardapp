using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.Ports.In;

public interface ILoginUseCase
{
    Task<Result<AuthenticationResult>> ExecuteAsync(LoginCommand command, CancellationToken cancellationToken = default);
}
