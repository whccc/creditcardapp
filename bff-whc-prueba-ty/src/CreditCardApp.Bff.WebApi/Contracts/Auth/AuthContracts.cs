namespace CreditCardApp.Bff.WebApi.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record SessionUserResponse(Guid Id, string FullName, string FirstName, string Initials, string Email);

public sealed record SessionResponse(DateTimeOffset ExpiresAt, SessionUserResponse User);
