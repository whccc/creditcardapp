using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Application.Ports.Out;

public interface ICardChargeGateway
{
    Task<Result<CardChargeOutcome>> ChargeAsync(CardChargeRequest request, CancellationToken cancellationToken);
}
