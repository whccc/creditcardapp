using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Domain;

public sealed record PaymentAmount
{
    public const decimal MaxValue = 1_000_000m;

    private PaymentAmount(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<PaymentAmount> Create(decimal value)
    {
        if (value <= 0 || value > MaxValue || decimal.Round(value, 2) != value)
        {
            return PaymentErrors.InvalidAmount;
        }

        return new PaymentAmount(value);
    }

    public static PaymentAmount FromPersistence(decimal value) => new(value);
}
