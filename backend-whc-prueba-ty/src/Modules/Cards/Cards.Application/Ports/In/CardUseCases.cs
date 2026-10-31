using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Application.Ports.In;

public interface IGetCardsUseCase
{
    Task<IReadOnlyList<CardDto>> ExecuteAsync(Guid ownerId, CancellationToken cancellationToken = default);
}

public interface IGetCardByIdUseCase
{
    Task<Result<CardDto>> ExecuteAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken = default);
}

public interface ICreateCardUseCase
{
    Task<Result<CardDto>> ExecuteAsync(CreateCardCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCardUseCase
{
    Task<Result<CardDto>> ExecuteAsync(UpdateCardCommand command, CancellationToken cancellationToken = default);
}

public interface IChangeCardStatusUseCase
{
    Task<Result<CardDto>> ExecuteAsync(ChangeCardStatusCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteCardUseCase
{
    Task<Result> ExecuteAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken = default);
}

public interface IChargeCardUseCase
{
    Task<Result<CardChargeReceipt>> ExecuteAsync(ChargeCardCommand command, CancellationToken cancellationToken = default);
}
