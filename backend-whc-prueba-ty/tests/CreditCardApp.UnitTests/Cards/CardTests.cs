using CreditCardApp.Modules.Cards.Domain;

namespace CreditCardApp.UnitTests.Cards;

public sealed class CardTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_StartsActiveWithFullAvailableCredit()
    {
        var card = CreateCard(creditLimit: 1_000m);

        Assert.Equal(CardStatus.Active, card.Status);
        Assert.Equal(1_000m, card.AvailableCredit);
        Assert.Equal(0m, card.UsedCredit);
        Assert.Equal("JANE DOE", card.HolderName);
    }

    [Fact]
    public void Create_WithExpiryInThePast_Fails()
    {
        var expiry = ExpiryDate.Create(9, 2026).Value;

        var result = Card.Create(Guid.NewGuid(), "Jane Doe", null, ValidNumber(), expiry, 1_000m, "USD", Now);

        Assert.Equal(CardErrors.ExpiryInThePast, result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(10.555)]
    [InlineData(1_000_001)]
    public void Create_WithInvalidCreditLimit_Fails(decimal creditLimit)
    {
        var result = Card.Create(Guid.NewGuid(), "Jane Doe", null, ValidNumber(), ValidExpiry(), creditLimit, "USD", Now);

        Assert.Equal(CardErrors.InvalidCreditLimit, result.Error);
    }

    [Fact]
    public void Charge_ReducesAvailableCredit()
    {
        var card = CreateCard(creditLimit: 1_000m);

        var result = card.Charge(250.50m, Now);

        Assert.True(result.IsSuccess);
        Assert.Equal(749.50m, card.AvailableCredit);
        Assert.Equal(250.50m, card.UsedCredit);
    }

    [Fact]
    public void Charge_AboveAvailableCredit_IsRejected()
    {
        var card = CreateCard(creditLimit: 100m);

        var result = card.Charge(100.01m, Now);

        Assert.Equal(CardErrors.InsufficientCredit, result.Error);
        Assert.Equal(100m, card.AvailableCredit);
    }

    [Fact]
    public void Charge_OnBlockedCard_IsRejected()
    {
        var card = CreateCard();
        card.Block(Now);

        var result = card.Charge(10m, Now);

        Assert.Equal(CardErrors.Blocked, result.Error);
    }

    [Fact]
    public void Charge_AfterExpiry_IsRejected()
    {
        var card = CreateCard();

        var result = card.Charge(10m, new DateTimeOffset(2031, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(CardErrors.Expired, result.Error);
    }

    [Fact]
    public void UpdateDetails_KeepsUsedCreditWhenLimitChanges()
    {
        var card = CreateCard(creditLimit: 1_000m);
        card.Charge(400m, Now);

        var result = card.UpdateDetails("Jane Doe", "Viajes", 2_000m, Now);

        Assert.True(result.IsSuccess);
        Assert.Equal(1_600m, card.AvailableCredit);
        Assert.Equal(400m, card.UsedCredit);
        Assert.Equal("Viajes", card.Nickname);
    }

    [Fact]
    public void UpdateDetails_BelowUsedCredit_Fails()
    {
        var card = CreateCard(creditLimit: 1_000m);
        card.Charge(400m, Now);

        var result = card.UpdateDetails("Jane Doe", null, 300m, Now);

        Assert.Equal(CardErrors.CreditLimitBelowUsage, result.Error);
        Assert.Equal(1_000m, card.CreditLimit);
    }

    [Fact]
    public void StateChanges_RotateTheConcurrencyVersion()
    {
        var card = CreateCard();
        var originalVersion = card.Version;

        card.Block(Now);

        Assert.NotEqual(originalVersion, card.Version);
    }

    private static Card CreateCard(decimal creditLimit = 1_000m) =>
        Card.Create(Guid.NewGuid(), " jane doe ", null, ValidNumber(), ValidExpiry(), creditLimit, "USD", Now).Value;

    private static CardNumber ValidNumber() => CardNumber.Create("4111111111111111").Value;

    private static ExpiryDate ValidExpiry() => ExpiryDate.Create(12, 2030).Value;
}
