using CreditCardApp.Modules.Authentication.Application.Contracts;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Authentication.Application.Ports.Out;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Authentication.Application.UseCases;

internal sealed class GetCurrentUserUseCase(IUserRepository users) : IGetCurrentUserUseCase
{
    public async Task<Result<UserDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);

        return user is null ? AuthenticationErrors.UserNotFound : UserDto.From(user);
    }
}
