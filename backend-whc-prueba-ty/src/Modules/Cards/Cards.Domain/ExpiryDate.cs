using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Domain;

public sealed class ExpiryDate
{
    private ExpiryDate(int month, int year)
    {
        Month = month;
        Year = year;
    }

    public int Month { get; private set; }

    public int Year { get; private set; }

    public static Result<ExpiryDate> Create(int month, int year)
    {
        if (month is < 1 or > 12 || year is < 2000 or > 2100)
        {
            return CardErrors.InvalidExpiry;
        }

        return new ExpiryDate(month, year);
    }

    public bool IsExpiredAt(DateTimeOffset now) =>
        Year < now.Year || (Year == now.Year && Month < now.Month);
}
