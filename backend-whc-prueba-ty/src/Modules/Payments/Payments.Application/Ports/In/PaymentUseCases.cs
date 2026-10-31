using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.SharedKernel.Pagination;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Application.Ports.In;

public interface IProcessPaymentUseCase
{
    Task<Result<PaymentDto>> ExecuteAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default);
}

public interface IGetPaymentHistoryUseCase
{
    Task<Result<PagedResult<PaymentDto>>> ExecuteAsync(PaymentHistoryQuery query, CancellationToken cancellationToken = default);
}

public interface IPaymentActivityQuery
{
    Task<bool> HasApprovedPaymentsSinceAsync(Guid userId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken = default);
}
