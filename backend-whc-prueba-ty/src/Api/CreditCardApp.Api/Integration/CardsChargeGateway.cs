using CreditCardApp.Modules.Cards.Application.Contracts;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Api.Integration;

internal sealed class CardsChargeGateway(
    IGetCardByIdUseCase getCard,
    IChargeCardUseCase chargeCard) : ICardChargeGateway
{
    public async Task<Result<CardChargeOutcome>> ChargeAsync(CardChargeRequest request, CancellationToken cancellationToken)
    {
        var card = await getCard.ExecuteAsync(request.UserId, request.CardId, cancellationToken);
        if (card.IsFailure)
        {
            return card.Error;
        }

        var charge = await chargeCard.ExecuteAsync(
            new ChargeCardCommand(request.UserId, request.CardId, request.Amount),
            cancellationToken);

        var (last4Digits, brand, currency) = (card.Value.Last4Digits, card.Value.Brand, card.Value.Currency);

        if (charge.IsSuccess)
        {
            return CardChargeOutcome.Approved(last4Digits, brand, currency);
        }

        return charge.Error.Type == ErrorType.BusinessRule
            ? CardChargeOutcome.Declined(last4Digits, brand, currency, charge.Error.Message)
            : charge.Error;
    }
}
