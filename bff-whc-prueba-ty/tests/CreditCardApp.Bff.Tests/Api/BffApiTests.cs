using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CreditCardApp.Bff.Tests.Support;

namespace CreditCardApp.Bff.Tests.Api;

public sealed class BffApiTests : IClassFixture<BffFactory>
{
    private const string CoreToken = "core-issued-jwt";

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid VisaId = Guid.NewGuid();
    private static readonly Guid AmexId = Guid.NewGuid();

    private readonly BffFactory _factory;

    public BffApiTests(BffFactory factory)
    {
        _factory = factory;
        _factory.CoreApi.Reset();
    }

    [Fact]
    public async Task Login_OpensAnHttpOnlySessionCookie_AndNeverReturnsTheToken()
    {
        var client = _factory.CreateBrowserClient();
        SetupCoreLogin();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "demo@creditcard.app", password = "Demo1234!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("UD", body.GetProperty("user").GetProperty("initials").GetString());
        Assert.False(body.TryGetProperty("accessToken", out _));
        Assert.DoesNotContain(CoreToken, await response.Content.ReadAsStringAsync());

        var cookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("ccapp.session=", cookie);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/api", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(CoreToken, cookie);
    }

    [Fact]
    public async Task Login_RelaysTheCoreProblemDetails_AndOpensNoSession()
    {
        var client = _factory.CreateBrowserClient();
        _factory.CoreApi.Setup(HttpMethod.Post, "/api/v1/auth/login", () =>
            StubCoreApi.Problem(HttpStatusCode.Unauthorized, "Auth.InvalidCredentials", "Correo o contraseña incorrectos."));

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "demo@creditcard.app", password = "nope" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Auth.InvalidCredentials", (await ReadJsonAsync(response)).GetProperty("code").GetString());
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Requests_WithoutTheCsrfHeader_AreRejected()
    {
        var client = _factory.CreateBrowserClient(withCsrfHeader: false);

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "demo@creditcard.app", password = "Demo1234!" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Bff.CsrfHeaderMissing", (await ReadJsonAsync(response)).GetProperty("code").GetString());
        Assert.Empty(_factory.CoreApi.Requests);
    }

    [Fact]
    public async Task Dashboard_WithoutSession_Returns401AndNeverCallsTheCore()
    {
        var response = await _factory.CreateBrowserClient().GetAsync("/api/v1/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Empty(_factory.CoreApi.Requests);
    }

    [Fact]
    public async Task Dashboard_ForwardsTheTokenFromTheCookie_AndAdaptsTheResponse()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(HttpMethod.Get, "/api/v1/users/me", () => StubCoreApi.Json(HttpStatusCode.OK, CoreUser()));
        _factory.CoreApi.Setup(HttpMethod.Get, "/api/v1/cards", () => StubCoreApi.Json(HttpStatusCode.OK, new[]
        {
            CoreCard(VisaId, "Visa", "1111", creditLimit: 5000m, availableCredit: 4500m, status: "Active"),
            CoreCard(AmexId, "Amex", "0005", creditLimit: 5000m, availableCredit: 5000m, status: "Blocked")
        }));

        var response = await client.GetAsync("/api/v1/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Usuario", body.GetProperty("user").GetProperty("firstName").GetString());

        var amex = body.GetProperty("cards")[1];
        Assert.Equal("•••• •••••• •0005", amex.GetProperty("maskedNumber").GetString());
        Assert.Equal("blocked", amex.GetProperty("status").GetString());

        var summary = body.GetProperty("summary");
        Assert.Equal(10000m, summary.GetProperty("totalCreditLimit").GetDecimal());
        Assert.Equal(1, summary.GetProperty("blockedCards").GetInt32());

        var coreCalls = _factory.CoreApi.Requests.Where(request => request.Path != "/api/v1/auth/login").ToList();
        Assert.Equal(2, coreCalls.Count);
        Assert.All(coreCalls, request => Assert.Equal($"Bearer {CoreToken}", request.Authorization));
    }

    [Fact]
    public async Task Session_ReturnsTheUserAndExpiration()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(HttpMethod.Get, "/api/v1/users/me", () => StubCoreApi.Json(HttpStatusCode.OK, CoreUser()));

        var response = await client.GetAsync("/api/v1/auth/session");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("demo@creditcard.app", body.GetProperty("user").GetProperty("email").GetString());
        Assert.True(body.GetProperty("expiresAt").GetDateTimeOffset() > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Logout_ClosesTheSession()
    {
        var client = await CreateLoggedInClientAsync();

        var logout = await client.PostAsync("/api/v1/auth/logout", null);
        var afterLogout = await client.GetAsync("/api/v1/dashboard");

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
    }

    [Fact]
    public async Task DeclinedPayment_IsAdaptedForTheUi()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(HttpMethod.Post, "/api/v1/payments", () => StubCoreApi.Json(HttpStatusCode.Created, new
        {
            id = Guid.NewGuid(),
            cardId = VisaId,
            cardLast4Digits = "4444",
            cardBrand = "Mastercard",
            amount = 5000m,
            currency = "USD",
            merchant = "Apple Store",
            status = "Declined",
            declineReason = "Cupo disponible insuficiente.",
            createdAt = DateTimeOffset.UtcNow
        }));

        var response = await client.PostAsJsonAsync("/api/v1/payments", new { cardId = VisaId, amount = 5000m, merchant = "Apple Store" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("declined", body.GetProperty("status").GetString());
        Assert.Equal("Mastercard •••• 4444", body.GetProperty("cardLabel").GetString());
    }

    [Fact]
    public async Task Payments_AreNeverRetried_AndACoreOutageReturns503()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(HttpMethod.Post, "/api/v1/payments", () => throw new HttpRequestException("Connection refused"));

        var response = await client.PostAsJsonAsync("/api/v1/payments", new { cardId = VisaId, amount = 10m, merchant = "Tienda" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Bff.DownstreamUnavailable", (await ReadJsonAsync(response)).GetProperty("code").GetString());
        Assert.Single(_factory.CoreApi.Requests, request => request.Path == "/api/v1/payments");
    }

    [Fact]
    public async Task Reads_AreRetriedWhenTheCoreFailsTransiently()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(
            HttpMethod.Get,
            "/api/v1/cards",
            () => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            () => StubCoreApi.Json(HttpStatusCode.OK, new[] { CoreCard(VisaId, "Visa", "1111", 1000m, 1000m, "Active") }));

        var response = await client.GetAsync("/api/v1/cards");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, _factory.CoreApi.Requests.Count(request => request.Path == "/api/v1/cards"));
    }

    [Fact]
    public async Task CoreServerErrors_AreReturnedAs502WithoutInternalDetails()
    {
        var client = await CreateLoggedInClientAsync();
        _factory.CoreApi.Setup(HttpMethod.Post, "/api/v1/cards", () =>
            StubCoreApi.Problem(HttpStatusCode.InternalServerError, "Internal", "NullReferenceException at CardRepository"));

        var response = await client.PostAsJsonAsync("/api/v1/cards", new
        {
            cardNumber = "4111111111111111",
            holderName = "Jane Doe",
            expiryMonth = 12,
            expiryYear = 2030,
            creditLimit = 1000m
        });

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        var problem = await ReadJsonAsync(response);
        Assert.Equal("Bff.DownstreamError", problem.GetProperty("code").GetString());
        Assert.DoesNotContain("NullReference", problem.GetProperty("detail").GetString());
    }

    private async Task<HttpClient> CreateLoggedInClientAsync()
    {
        var client = _factory.CreateBrowserClient();
        SetupCoreLogin();

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "demo@creditcard.app", password = "Demo1234!" });
        login.EnsureSuccessStatusCode();

        return client;
    }

    private void SetupCoreLogin() =>
        _factory.CoreApi.Setup(HttpMethod.Post, "/api/v1/auth/login", () => StubCoreApi.Json(HttpStatusCode.OK, new
        {
            accessToken = CoreToken,
            expiresAt = DateTimeOffset.UtcNow.AddHours(1),
            user = CoreUser()
        }));

    private static object CoreUser() => new { id = UserId, email = "demo@creditcard.app", fullName = "Usuario Demo" };

    private static object CoreCard(Guid id, string brand, string last4Digits, decimal creditLimit, decimal availableCredit, string status) => new
    {
        id,
        holderName = "USUARIO DEMO",
        nickname = (string?)null,
        last4Digits,
        brand,
        expiryMonth = 8,
        expiryYear = 2029,
        creditLimit,
        availableCredit,
        usedCredit = creditLimit - availableCredit,
        currency = "USD",
        status,
        isExpired = false,
        createdAt = DateTimeOffset.UtcNow
    };

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>();
}
