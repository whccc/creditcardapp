namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class Percentage
{
    public static decimal Of(decimal part, decimal total) =>
        total <= 0 ? 0 : Math.Round(part / total * 100, 1, MidpointRounding.AwayFromZero);
}
