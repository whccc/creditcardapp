using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Domain;
using CreditCardApp.SharedKernel.Pagination;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Application.UseCases;

internal sealed class GetPaymentHistoryUseCase(IPaymentRepository payments) : IGetPaymentHistoryUseCase
{
    public async Task<Result<PagedResult<PaymentDto>>> ExecuteAsync(PaymentHistoryQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page < 1 || query.PageSize is < 1 or > PaymentHistoryQuery.MaxPageSize)
        {
            return PaymentErrors.InvalidPagination;
        }

        var (items, totalCount) = await payments.GetPageAsync(
            query.UserId,
            query.CardId,
            (query.Page - 1) * query.PageSize,
            query.PageSize,
            cancellationToken);

        return new PagedResult<PaymentDto>(
            items.Select(PaymentDto.From).ToList(),
            query.Page,
            query.PageSize,
            totalCount);
    }
}
