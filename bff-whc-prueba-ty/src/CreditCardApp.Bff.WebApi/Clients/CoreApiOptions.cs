using System.ComponentModel.DataAnnotations;

namespace CreditCardApp.Bff.WebApi.Clients;

internal sealed class CoreApiOptions
{
    public const string SectionName = "CoreApi";

    [Required]
    public Uri AuthenticationBaseUrl { get; init; } = null!;

    [Required]
    public Uri CardsBaseUrl { get; init; } = null!;

    [Required]
    public Uri PaymentsBaseUrl { get; init; } = null!;
}
