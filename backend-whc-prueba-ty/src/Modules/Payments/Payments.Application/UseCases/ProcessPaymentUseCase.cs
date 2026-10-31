using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Application.UseCases;

internal sealed class ProcessPaymentUseCase(
    ICardChargeGateway cardChargeGateway,
    IPaymentRepository payments,
    TimeProvider timeProvider) : IProcessPaymentUseCase
{
    public async Task<Result<PaymentDto>> ExecuteAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var amount = PaymentAmount.Create(command.Amount);
        if (amount.IsFailure)
        {
            return amount.Error;
        }

        var merchant = Merchant.Create(command.Merchant);
        if (merchant.IsFailure)
        {
            return merchant.Error;
        }

        var charge = await cardChargeGateway.ChargeAsync(
            new CardChargeRequest(command.UserId, command.CardId, amount.Value.Value),
            cancellationToken);

        if (charge.IsFailure)
        {
            return charge.Error;
        }

        var outcome = charge.Value;
        var card = new CardSnapshot(command.CardId, outcome.CardLast4Digits, outcome.CardBrand, outcome.Currency);
        var now = timeProvider.GetUtcNow();

        var payment = outcome.IsApproved
            ? Payment.Approve(command.UserId, card, amount.Value, merchant.Value, now)
            : Payment.Decline(command.UserId, card, amount.Value, merchant.Value, outcome.DeclineReason, now);

        payments.Add(payment);
        await payments.SaveChangesAsync(cancellationToken);

        return PaymentDto.From(payment);
    }
}
