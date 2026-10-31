using CreditCardApp.Bff.WebApi.Clients;
using CreditCardApp.Bff.WebApi.Contracts.Common;
using CreditCardApp.Bff.WebApi.Contracts.Payments;
using CreditCardApp.Bff.WebApi.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Controllers;

[Route($"{RoutePrefix}/payments")]
[Tags("Payments")]
public sealed class PaymentsController(PaymentsApiClient paymentsApi) : ApiControllerBase
{
    private const int DefaultPageSize = 10;

    [HttpPost]
    [EndpointSummary("Procesa un pago con una tarjeta. Devuelve el pago con estado approved o declined.")]
    [ProducesResponseType<PaymentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PaymentResponse>> Process(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await paymentsApi.ProcessAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetHistory), new { cardId = result.Value.CardId }, result.Value.ToResponse())
            : Relay(result);
    }

    [HttpGet]
    [EndpointSummary("Historial paginado de pagos, opcionalmente filtrado por tarjeta.")]
    [ProducesResponseType<PagedResponse<PaymentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<PaymentResponse>>> GetHistory(
        [FromQuery] Guid? cardId,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var result = await paymentsApi.GetHistoryAsync(cardId, page, pageSize, cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponse()) : Relay(result);
    }
}
