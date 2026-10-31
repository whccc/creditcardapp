using CreditCardApp.Modules.Payments.Application.Contracts;
using CreditCardApp.Modules.Payments.Application.Ports.Out;
using CreditCardApp.Modules.Payments.Application.UseCases;
using CreditCardApp.Modules.Payments.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.UnitTests.Payments;

public sealed class ProcessPaymentUseCaseTests
{
    private readonly FakeCardChargeGateway _gateway = new();
    private readonly InMemoryPaymentRepository _payments = new();
    private readonly ProcessPaymentUseCase _useCase;

    public ProcessPaymentUseCaseTests() =>
        _useCase = new ProcessPaymentUseCase(_gateway, _payments, TimeProvider.System);

    [Fact]
    public async Task ApprovedCharge_RecordsApprovedPayment()
    {
        _gateway.NextResult = CardChargeOutcome.Approved("1111", "Visa", "USD");

        var result = await _useCase.ExecuteAsync(Command(amount: 99.90m, merchant: "  Netflix "));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsApproved);
        Assert.Equal("Netflix", result.Value.Merchant);
        Assert.Equal(99.90m, result.Value.Amount);
        Assert.Single(_payments.Saved);
    }

    [Fact]
    public async Task DeclinedCharge_RecordsDeclinedPaymentWithReason()
    {
        _gateway.NextResult = CardChargeOutcome.Declined("4444", "Mastercard", "USD", "Cupo disponible insuficiente.");

        var result = await _useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Equal(nameof(PaymentStatus.Declined), result.Value.Status);
        Assert.Equal("Cupo disponible insuficiente.", result.Value.DeclineReason);
        Assert.Single(_payments.Saved);
    }

    [Fact]
    public async Task UnknownCard_PropagatesErrorAndRecordsNothing()
    {
        var notFound = Error.NotFound("Card.NotFound", "La tarjeta no existe.");
        _gateway.NextResult = notFound;

        var result = await _useCase.ExecuteAsync(Command());

        Assert.Equal(notFound, result.Error);
        Assert.Empty(_payments.Saved);
    }

    [Theory]
    [InlineData(0, "Netflix")]
    [InlineData(-5, "Netflix")]
    [InlineData(10.123, "Netflix")]
    [InlineData(10, "")]
    [InlineData(10, "N")]
    public async Task InvalidInput_IsRejectedBeforeChargingTheCard(decimal amount, string merchant)
    {
        var result = await _useCase.ExecuteAsync(Command(amount, merchant));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal(0, _gateway.Calls);
    }

    private static ProcessPaymentCommand Command(decimal amount = 10m, string merchant = "Netflix") =>
        new(Guid.NewGuid(), Guid.NewGuid(), amount, merchant);

    private sealed class FakeCardChargeGateway : ICardChargeGateway
    {
        public Result<CardChargeOutcome> NextResult { get; set; } = CardChargeOutcome.Approved("1111", "Visa", "USD");

        public int Calls { get; private set; }

        public Task<Result<CardChargeOutcome>> ChargeAsync(CardChargeRequest request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(NextResult);
        }
    }

    private sealed class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _pending = [];

        public List<Payment> Saved { get; } = [];

        public void Add(Payment payment) => _pending.Add(payment);

        public Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPageAsync(
            Guid userId,
            Guid? cardId,
            int skip,
            int take,
            CancellationToken cancellationToken) =>
            Task.FromResult<(IReadOnlyList<Payment>, int)>((Saved.Skip(skip).Take(take).ToList(), Saved.Count));

        public Task<bool> HasApprovedPaymentsSinceAsync(Guid userId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken) =>
            Task.FromResult(Saved.Any(payment =>
                payment.UserId == userId
                && payment.CardId == cardId
                && payment.Status == PaymentStatus.Approved
                && payment.CreatedAt >= since));

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            Saved.AddRange(_pending);
            _pending.Clear();
            return Task.CompletedTask;
        }
    }
}
