using CreditCardApp.Modules.Cards.Application.Ports.Out;
using CreditCardApp.Modules.Cards.Application.UseCases;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.UnitTests.Cards;

public sealed class DeleteCardUseCaseTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    private readonly InMemoryCardRepository _cards = new();
    private readonly FakeCardPaymentActivity _paymentActivity = new();
    private readonly DeleteCardUseCase _useCase;

    public DeleteCardUseCaseTests() =>
        _useCase = new DeleteCardUseCase(_cards, _paymentActivity, new FixedTimeProvider(Now));

    [Fact]
    public async Task CardWithRecentApprovedPayments_IsNotDeleted()
    {
        var card = _cards.Seed(CreateCard());
        _paymentActivity.HasRecentPayments = true;

        var result = await _useCase.ExecuteAsync(card.OwnerId, card.Id);

        Assert.Equal(CardErrors.HasRecentPayments, result.Error);
        Assert.Equal(ErrorType.BusinessRule, result.Error.Type);
        Assert.Contains(card, _cards.Stored);
    }

    [Fact]
    public async Task CardWithoutRecentApprovedPayments_IsDeleted()
    {
        var card = _cards.Seed(CreateCard());

        var result = await _useCase.ExecuteAsync(card.OwnerId, card.Id);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(card, _cards.Stored);
    }

    [Fact]
    public async Task PaymentActivity_IsCheckedForTheLast30Days()
    {
        var card = _cards.Seed(CreateCard());

        await _useCase.ExecuteAsync(card.OwnerId, card.Id);

        Assert.Equal((card.OwnerId, card.Id, Now.AddDays(-30)), _paymentActivity.LastQuery);
    }

    [Fact]
    public async Task UnknownCard_ReturnsNotFoundWithoutAskingPayments()
    {
        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(CardErrors.NotFound, result.Error);
        Assert.Null(_paymentActivity.LastQuery);
    }

    private static Card CreateCard() =>
        Card.Create(
            Guid.NewGuid(),
            "Jane Doe",
            null,
            CardNumber.Create("4111111111111111").Value,
            ExpiryDate.Create(12, 2030).Value,
            1_000m,
            "USD",
            Now).Value;

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeCardPaymentActivity : ICardPaymentActivity
    {
        public bool HasRecentPayments { get; set; }

        public (Guid OwnerId, Guid CardId, DateTimeOffset Since)? LastQuery { get; private set; }

        public Task<bool> HasApprovedPaymentsSinceAsync(Guid ownerId, Guid cardId, DateTimeOffset since, CancellationToken cancellationToken)
        {
            LastQuery = (ownerId, cardId, since);
            return Task.FromResult(HasRecentPayments);
        }
    }

    private sealed class InMemoryCardRepository : ICardRepository
    {
        public List<Card> Stored { get; } = [];

        public Card Seed(Card card)
        {
            Stored.Add(card);
            return card;
        }

        public Task<Card?> GetAsync(Guid ownerId, Guid cardId, CancellationToken cancellationToken) =>
            Task.FromResult(Stored.SingleOrDefault(card => card.OwnerId == ownerId && card.Id == cardId));

        public Task<IReadOnlyList<Card>> ListAsync(Guid ownerId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Card>>(Stored.Where(card => card.OwnerId == ownerId).ToList());

        public void Add(Card card) => Stored.Add(card);

        public void Remove(Card card) => Stored.Remove(card);

        public Task<Result> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(Result.Success());
    }
}
