using CreditCardApp.Bff.WebApi.Clients.Contracts;
using CreditCardApp.Bff.WebApi.Mapping;

namespace CreditCardApp.Bff.Tests.Mapping;

public sealed class MappingTests
{
    [Theory]
    [InlineData("Visa", "1111", "•••• •••• •••• 1111", "visa", "Visa")]
    [InlineData("Mastercard", "4444", "•••• •••• •••• 4444", "mastercard", "Mastercard")]
    [InlineData("Amex", "0005", "•••• •••••• •0005", "amex", "American Express")]
    public void Card_IsMaskedAndLabelledPerBrand(string brand, string last4, string expectedMask, string expectedBrand, string expectedName)
    {
        var response = Card(brand, last4).ToResponse();

        Assert.Equal(expectedMask, response.MaskedNumber);
        Assert.Equal(expectedBrand, response.Brand);
        Assert.Equal(expectedName, response.BrandName);
    }

    [Fact]
    public void Card_FormatsExpiryStatusAndUtilization()
    {
        var response = Card("Visa", "1111", creditLimit: 3000m, availableCredit: 2739.02m, status: "Blocked").ToResponse();

        Assert.Equal("08/29", response.Expiry);
        Assert.Equal("blocked", response.Status);
        Assert.Equal(8.7m, response.UtilizationPercentage);
    }

    [Theory]
    [InlineData("Usuario Demo", "Usuario", "UD")]
    [InlineData("  maría   josé  pérez ", "maría", "MJ")]
    [InlineData("Cher", "Cher", "C")]
    public void User_GetsFirstNameAndInitials(string fullName, string expectedFirstName, string expectedInitials)
    {
        var response = new UserDto(Guid.NewGuid(), "x@y.z", fullName).ToResponse();

        Assert.Equal(expectedFirstName, response.FirstName);
        Assert.Equal(expectedInitials, response.Initials);
    }

    [Fact]
    public void Summary_OfNoCards_IsZeroWithDefaultCurrency()
    {
        var summary = Array.Empty<CardDto>().ToResponses().ToSummary();

        Assert.Equal(0m, summary.UtilizationPercentage);
        Assert.Equal("USD", summary.Currency);
    }

    private static CardDto Card(string brand, string last4, decimal creditLimit = 1000m, decimal availableCredit = 1000m, string status = "Active") =>
        new(Guid.NewGuid(), "USUARIO DEMO", null, last4, brand, 8, 2029, creditLimit, availableCredit, creditLimit - availableCredit, "USD", status, false, DateTimeOffset.UtcNow);
}
