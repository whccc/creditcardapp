using System.ComponentModel.DataAnnotations;

namespace CreditCardApp.Modules.Cards.Application;

public sealed class CardsOptions
{
    public const string SectionName = "Cards";

    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; init; } = "USD";
}
