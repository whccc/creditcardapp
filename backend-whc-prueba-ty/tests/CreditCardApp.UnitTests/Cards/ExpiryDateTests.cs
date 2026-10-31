using CreditCardApp.Modules.Cards.Domain;

namespace CreditCardApp.UnitTests.Cards;

public sealed class ExpiryDateTests
{
    [Theory]
    [InlineData(0, 2030)]
    [InlineData(13, 2030)]
    [InlineData(5, 1999)]
    public void Create_WithOutOfRangeValues_Fails(int month, int year)
    {
        Assert.Equal(CardErrors.InvalidExpiry, ExpiryDate.Create(month, year).Error);
    }

    [Theory]
    [InlineData(10, 2026, false)]
    [InlineData(9, 2026, true)]
    [InlineData(1, 2027, false)]
    [InlineData(12, 2025, true)]
    public void IsExpiredAt_TreatsTheCardAsValidThroughTheEndOfItsMonth(int month, int year, bool expectedExpired)
    {
        var now = new DateTimeOffset(2026, 10, 31, 23, 59, 0, TimeSpan.Zero);

        Assert.Equal(expectedExpired, ExpiryDate.Create(month, year).Value.IsExpiredAt(now));
    }
}
