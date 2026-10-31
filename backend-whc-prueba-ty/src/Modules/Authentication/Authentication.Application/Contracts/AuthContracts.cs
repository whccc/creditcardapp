namespace CreditCardApp.Modules.Authentication.Application.Contracts;

public sealed record LoginCommand(string Email, string Password);

public sealed record RegisterUserCommand(string Email, string FullName, string Password);

public sealed record AuthenticationResult(string AccessToken, DateTimeOffset ExpiresAt, UserDto User);
