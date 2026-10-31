using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Domain;

public sealed class CardNumber
{
    private readonly string _digits;

    private CardNumber(string digits, CardBrand brand)
    {
        _digits = digits;
        Brand = brand;
    }

    public CardBrand Brand { get; }

    public string Last4Digits => _digits[^4..];

    public static Result<CardNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CardErrors.InvalidNumber;
        }

        var digits = string.Concat(value.Where(character => character is not (' ' or '-')));

        if (digits.Length is < 13 or > 19 || !digits.All(char.IsAsciiDigit) || !PassesLuhnCheck(digits))
        {
            return CardErrors.InvalidNumber;
        }

        var brand = DetectBrand(digits);

        return brand is null ? CardErrors.UnsupportedBrand : new CardNumber(digits, brand.Value);
    }

    public override string ToString() => $"**** {Last4Digits}";

    private static CardBrand? DetectBrand(string digits)
    {
        var firstTwo = int.Parse(digits.AsSpan(0, 2));
        var firstFour = int.Parse(digits.AsSpan(0, 4));

        return digits switch
        {
            { Length: 13 or 16 or 19 } when digits[0] == '4' => CardBrand.Visa,
            { Length: 16 } when firstTwo is >= 51 and <= 55 || firstFour is >= 2221 and <= 2720 => CardBrand.Mastercard,
            { Length: 15 } when firstTwo is 34 or 37 => CardBrand.Amex,
            _ => null
        };
    }

    private static bool PassesLuhnCheck(string digits)
    {
        var sum = 0;
        var doubleDigit = false;

        for (var index = digits.Length - 1; index >= 0; index--)
        {
            var digit = digits[index] - '0';

            if (doubleDigit)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            doubleDigit = !doubleDigit;
        }

        return sum % 10 == 0;
    }
}
