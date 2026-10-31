namespace CreditCardApp.Bff.WebApi.Mapping;

internal static class BrandNames
{
    public const string Visa = "Visa";
    public const string Mastercard = "Mastercard";
    public const string Amex = "Amex";

    public static string For(string brand) => brand switch
    {
        Visa => "Visa",
        Mastercard => "Mastercard",
        Amex => "American Express",
        _ => brand
    };
}
