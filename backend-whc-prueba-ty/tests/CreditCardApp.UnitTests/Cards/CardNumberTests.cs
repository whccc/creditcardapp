using CreditCardApp.Modules.Cards.Domain;

namespace CreditCardApp.UnitTests.Cards;

public sealed class CardNumberTests
{
    [Theory]
    [InlineData("4111 1111 1111 1111", "Visa", "1111")]
    [InlineData("5555-5555-5555-4444", "Mastercard", "4444")]
    [InlineData("2223003122003222", "Mastercard", "3222")]
    [InlineData("378282246310005", "Amex", "0005")]
    public void Create_WithValidNumber_DetectsBrandAndLast4Digits(string value, string expectedBrand, string expectedLast4)
    {
        var result = CardNumber.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedBrand, result.Value.Brand.ToString());
        Assert.Equal(expectedLast4, result.Value.Last4Digits);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("4111 1111 1111 1112")]
    [InlineData("4111-abcd-1111-1111")]
    [InlineData("411111")]
    public void Create_WithInvalidNumber_ReturnsInvalidNumber(string? value)
    {
        var result = CardNumber.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(CardErrors.InvalidNumber, result.Error);
    }

    [Fact]
    public void Create_WithUnsupportedBrand_ReturnsUnsupportedBrand()
    {
        var result = CardNumber.Create("6011111111111117");

        Assert.Equal(CardErrors.UnsupportedBrand, result.Error);
    }

    [Fact]
    public void ToString_NeverExposesTheFullNumber()
    {
        var number = CardNumber.Create("4111111111111111").Value;

        Assert.Equal("**** 1111", number.ToString());
    }
}
