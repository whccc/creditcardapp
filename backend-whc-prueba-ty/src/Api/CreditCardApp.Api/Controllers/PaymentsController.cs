using CreditCardApp.Api.Contracts.Payments;
using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.SharedKernel.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Api.Controllers;

[Route($"{RoutePrefix}/payments")]
[Tags("Payments")]
public sealed class PaymentsController(
    IProcessPaymentUseCase processPayment,
    IGetPaymentHistoryUseCase getPaymentHistory) : ApiControllerBase
{
    private const int DefaultPageSize = 10;

    [HttpPost]
    [EndpointSummary("Procesa un pago con una tarjeta. Devuelve el pago con estado Approved o Declined.")]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PaymentDto>> Process(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(CurrentUserId, request.CardId, request.Amount, request.Merchant);
        var result = await processPayment.ExecuteAsync(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetHistory), new { cardId = result.Value.CardId }, result.Value)
            : ErrorResult(result.Error);
    }

    [HttpGet]
    [EndpointSummary("Historial paginado de pagos, opcionalmente filtrado por tarjeta.")]
    [ProducesResponseType<PagedResult<PaymentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PaymentDto>>> GetHistory(
        [FromQuery] Guid? cardId,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var result = await getPaymentHistory.ExecuteAsync(
            new PaymentHistoryQuery(CurrentUserId, cardId, page, pageSize),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }
}
