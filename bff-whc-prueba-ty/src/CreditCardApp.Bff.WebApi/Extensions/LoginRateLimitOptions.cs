using System.ComponentModel.DataAnnotations;

namespace CreditCardApp.Bff.WebApi.Extensions;

internal sealed class LoginRateLimitOptions
{
    public const string SectionName = "RateLimiting:Login";
    public const string PolicyName = "login";

    [Range(1, 10_000)]
    public int PermitLimit { get; init; } = 5;

    [Range(1, 3_600)]
    public int WindowSeconds { get; init; } = 60;
}
