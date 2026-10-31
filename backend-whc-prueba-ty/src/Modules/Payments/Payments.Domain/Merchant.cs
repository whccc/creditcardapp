using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Domain;

public sealed record Merchant
{
    public const int MinLength = 2;
    public const int MaxLength = 80;

    private Merchant(string name) => Name = name;

    public string Name { get; }

    public static Result<Merchant> Create(string? name)
    {
        var normalized = name?.Trim() ?? string.Empty;

        if (normalized.Length is < MinLength or > MaxLength)
        {
            return PaymentErrors.InvalidMerchant;
        }

        return new Merchant(normalized);
    }

    public static Merchant FromPersistence(string name) => new(name);
}
