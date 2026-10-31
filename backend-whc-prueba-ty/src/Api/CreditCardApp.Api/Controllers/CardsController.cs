using CreditCardApp.Api.Contracts.Cards;
using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Api.Controllers;

[Route($"{RoutePrefix}/cards")]
[Tags("Cards")]
public sealed class CardsController(
    IGetCardsUseCase getCards,
    IGetCardByIdUseCase getCard,
    ICreateCardUseCase createCard,
    IUpdateCardUseCase updateCard,
    IChangeCardStatusUseCase changeCardStatus,
    IDeleteCardUseCase deleteCard) : ApiControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista las tarjetas del usuario.")]
    [ProducesResponseType<IReadOnlyList<CardDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CardDto>>> GetAll(CancellationToken cancellationToken)
    {
        var cards = await getCards.ExecuteAsync(CurrentUserId, cancellationToken);

        return Ok(cards);
    }

    [HttpGet("{cardId:guid}")]
    [EndpointSummary("Obtiene una tarjeta.")]
    [ProducesResponseType<CardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> GetById(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await getCard.ExecuteAsync(CurrentUserId, cardId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }

    [HttpPost]
    [EndpointSummary("Registra una nueva tarjeta.")]
    [ProducesResponseType<CardDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CardDto>> Create(CreateCardRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCardCommand(
            CurrentUserId,
            request.CardNumber,
            request.HolderName,
            request.Nickname,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.CreditLimit);

        var result = await createCard.ExecuteAsync(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { cardId = result.Value.Id }, result.Value)
            : ErrorResult(result.Error);
    }

    [HttpPut("{cardId:guid}")]
    [EndpointSummary("Actualiza titular, alias y cupo.")]
    [ProducesResponseType<CardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CardDto>> Update(Guid cardId, UpdateCardRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCardCommand(CurrentUserId, cardId, request.HolderName, request.Nickname, request.CreditLimit);
        var result = await updateCard.ExecuteAsync(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }

    [HttpPost("{cardId:guid}/block")]
    [EndpointSummary("Bloquea la tarjeta.")]
    [ProducesResponseType<CardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ActionResult<CardDto>> Block(Guid cardId, CancellationToken cancellationToken) =>
        ChangeStatusAsync(cardId, block: true, cancellationToken);

    [HttpPost("{cardId:guid}/unblock")]
    [EndpointSummary("Desbloquea la tarjeta.")]
    [ProducesResponseType<CardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ActionResult<CardDto>> Unblock(Guid cardId, CancellationToken cancellationToken) =>
        ChangeStatusAsync(cardId, block: false, cancellationToken);

    [HttpDelete("{cardId:guid}")]
    [EndpointSummary("Elimina la tarjeta. Su historial de pagos se conserva.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await deleteCard.ExecuteAsync(CurrentUserId, cardId, cancellationToken);

        return result.IsSuccess ? NoContent() : ErrorResult(result.Error);
    }

    private async Task<ActionResult<CardDto>> ChangeStatusAsync(Guid cardId, bool block, CancellationToken cancellationToken)
    {
        var result = await changeCardStatus.ExecuteAsync(new ChangeCardStatusCommand(CurrentUserId, cardId, block), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error);
    }
}
