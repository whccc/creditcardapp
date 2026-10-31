using CreditCardApp.Bff.WebApi.Clients;
using CreditCardApp.Bff.WebApi.Contracts.Dashboard;
using CreditCardApp.Bff.WebApi.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Controllers;

[Route($"{RoutePrefix}/dashboard")]
[Tags("Dashboard")]
public sealed class DashboardController(AuthApiClient authApi, CardsApiClient cardsApi) : ApiControllerBase
{
    [HttpGet]
    [EndpointSummary("Agrega en una sola llamada el usuario, sus tarjetas y el resumen de cupos.")]
    [ProducesResponseType<DashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken cancellationToken)
    {
        var userTask = authApi.GetCurrentUserAsync(cancellationToken);
        var cardsTask = cardsApi.GetAllAsync(cancellationToken);
        await Task.WhenAll(userTask, cardsTask);

        var user = await userTask;
        if (!user.IsSuccess)
        {
            return Relay(user);
        }

        var cards = await cardsTask;
        if (!cards.IsSuccess)
        {
            return Relay(cards);
        }

        var cardResponses = cards.Value.ToResponses();

        return Ok(new DashboardResponse(user.Value.ToResponse(), cardResponses.ToSummary(), cardResponses));
    }
}
