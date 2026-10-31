using CreditCardApp.Bff.WebApi.Clients;
using CreditCardApp.Bff.WebApi.Contracts.Cards;
using CreditCardApp.Bff.WebApi.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Controllers;

[Route($"{RoutePrefix}/cards")]
[Tags("Cards")]
public sealed class CardsController(CardsApiClient cardsApi) : ApiControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista las tarjetas del usuario.")]
    [ProducesResponseType<IReadOnlyList<CardResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CardResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await cardsApi.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponses()) : Relay(result);
    }

    [HttpGet("{cardId:guid}")]
    [EndpointSummary("Obtiene una tarjeta.")]
    [ProducesResponseType<CardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardResponse>> GetById(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await cardsApi.GetByIdAsync(cardId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponse()) : Relay(result);
    }

    [HttpPost]
    [EndpointSummary("Registra una nueva tarjeta.")]
    [ProducesResponseType<CardResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CardResponse>> Create(CreateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await cardsApi.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { cardId = result.Value.Id }, result.Value.ToResponse())
            : Relay(result);
    }

    [HttpPut("{cardId:guid}")]
    [EndpointSummary("Actualiza titular, alias y cupo.")]
    [ProducesResponseType<CardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CardResponse>> Update(Guid cardId, UpdateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await cardsApi.UpdateAsync(cardId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponse()) : Relay(result);
    }

    [HttpPost("{cardId:guid}/block")]
    [EndpointSummary("Bloquea la tarjeta.")]
    [ProducesResponseType<CardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardResponse>> Block(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await cardsApi.BlockAsync(cardId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponse()) : Relay(result);
    }

    [HttpPost("{cardId:guid}/unblock")]
    [EndpointSummary("Desbloquea la tarjeta.")]
    [ProducesResponseType<CardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardResponse>> Unblock(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await cardsApi.UnblockAsync(cardId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value.ToResponse()) : Relay(result);
    }

    [HttpDelete("{cardId:guid}")]
    [EndpointSummary("Elimina la tarjeta. Su historial de pagos se conserva.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await cardsApi.DeleteAsync(cardId, cancellationToken);

        return result.IsSuccess ? NoContent() : Relay(result);
    }
}
