namespace CreditCardApp.Api.Seeding;

internal sealed class DemoDataOptions
{
    public const string SectionName = "DemoData";

    public bool Enabled { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;
}
