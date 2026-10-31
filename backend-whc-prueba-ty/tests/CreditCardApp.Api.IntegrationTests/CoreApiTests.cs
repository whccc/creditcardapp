using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CreditCardApp.Api.IntegrationTests;

public sealed class CoreApiTests(CoreApiFactory factory) : IClassFixture<CoreApiFactory>
{
    [Fact]
    public async Task Login_WithSeededDemoUser_ReturnsTokenAndUser()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = CoreApiFactory.DemoEmail,
            password = CoreApiFactory.DemoPassword
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("accessToken").GetString()));
        Assert.Equal("Usuario Demo", body.GetProperty("user").GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsProblemDetails401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = CoreApiFactory.DemoEmail, password = "nope" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Auth.InvalidCredentials", (await ReadJsonAsync(response)).GetProperty("code").GetString());
    }

    [Theory]
    [InlineData("/api/v1/users/me")]
    [InlineData("/api/v1/cards")]
    [InlineData("/api/v1/payments")]
    public async Task ProtectedEndpoints_WithoutToken_Return401(string url)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CurrentUser_ReturnsTheTokenOwner()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/v1/users/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(CoreApiFactory.DemoEmail, (await ReadJsonAsync(response)).GetProperty("email").GetString());
    }

    [Fact]
    public async Task Cards_ReturnsSeededCardsWithRawModuleData()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/v1/cards");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cards = (await ReadJsonAsync(response)).EnumerateArray().ToList();
        Assert.True(cards.Count >= 3);
        Assert.Contains(cards, card => card.GetProperty("status").GetString() == "Blocked");
        Assert.All(cards, card => Assert.False(card.TryGetProperty("maskedNumber", out _)));
        Assert.All(cards, card => Assert.Equal(4, card.GetProperty("last4Digits").GetString()!.Length));
    }

    [Fact]
    public async Task CardLifecycle_CreateUpdateBlockDelete()
    {
        var client = await CreateAuthenticatedClientAsync();

        var created = await client.PostAsJsonAsync("/api/v1/cards", new
        {
            cardNumber = "4012 8888 8888 1881",
            holderName = "Jane Doe",
            nickname = "Prueba",
            expiryMonth = 12,
            expiryYear = DateTime.UtcNow.Year + 2,
            creditLimit = 1500m
        });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.NotNull(created.Headers.Location);
        var card = await ReadJsonAsync(created);
        var cardId = card.GetProperty("id").GetGuid();
        Assert.Equal("Visa", card.GetProperty("brand").GetString());
        Assert.Equal("1881", card.GetProperty("last4Digits").GetString());
        Assert.Equal("JANE DOE", card.GetProperty("holderName").GetString());

        var updated = await client.PutAsJsonAsync($"/api/v1/cards/{cardId}", new { holderName = "Jane Doe", nickname = "Editada", creditLimit = 2500m });
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        Assert.Equal(2500m, (await ReadJsonAsync(updated)).GetProperty("availableCredit").GetDecimal());

        var blocked = await client.PostAsync($"/api/v1/cards/{cardId}/block", null);
        Assert.Equal("Blocked", (await ReadJsonAsync(blocked)).GetProperty("status").GetString());

        var deleted = await client.DeleteAsync($"/api/v1/cards/{cardId}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var afterDelete = await client.GetAsync($"/api/v1/cards/{cardId}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task CreateCard_WithInvalidLuhnNumber_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/cards", new
        {
            cardNumber = "4111 1111 1111 1112",
            holderName = "Jane Doe",
            expiryMonth = 12,
            expiryYear = DateTime.UtcNow.Year + 2,
            creditLimit = 1000m
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Card.InvalidNumber", (await ReadJsonAsync(response)).GetProperty("code").GetString());
    }

    [Fact]
    public async Task Payments_ApproveDeclineAndAppearInHistory()
    {
        var client = await CreateAuthenticatedClientAsync();
        var cardId = await CreateCardAsync(client, creditLimit: 100m);

        var approved = await client.PostAsJsonAsync("/api/v1/payments", new { cardId, amount = 60m, merchant = "Cafetería" });
        Assert.Equal(HttpStatusCode.Created, approved.StatusCode);
        Assert.Equal("Approved", (await ReadJsonAsync(approved)).GetProperty("status").GetString());

        var declined = await client.PostAsJsonAsync("/api/v1/payments", new { cardId, amount = 60m, merchant = "Librería" });
        var declinedBody = await ReadJsonAsync(declined);
        Assert.Equal("Declined", declinedBody.GetProperty("status").GetString());
        Assert.Equal("Cupo disponible insuficiente.", declinedBody.GetProperty("declineReason").GetString());

        var card = await ReadJsonAsync(await client.GetAsync($"/api/v1/cards/{cardId}"));
        Assert.Equal(40m, card.GetProperty("availableCredit").GetDecimal());

        var history = await ReadJsonAsync(await client.GetAsync($"/api/v1/payments?cardId={cardId}&page=1&pageSize=10"));
        Assert.Equal(2, history.GetProperty("totalCount").GetInt32());
        Assert.Equal("Librería", history.GetProperty("items")[0].GetProperty("merchant").GetString());
    }

    [Fact]
    public async Task Payment_OnAnotherUsersOrUnknownCard_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/payments", new { cardId = Guid.NewGuid(), amount = 10m, merchant = "Tienda" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PaymentHistory_WithInvalidPageSize_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/v1/payments?pageSize=500");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = CoreApiFactory.DemoEmail,
            password = CoreApiFactory.DemoPassword
        });
        login.EnsureSuccessStatusCode();

        var token = (await ReadJsonAsync(login)).GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static async Task<Guid> CreateCardAsync(HttpClient client, decimal creditLimit)
    {
        var response = await client.PostAsJsonAsync("/api/v1/cards", new
        {
            cardNumber = "5105 1051 0510 5100",
            holderName = "Jane Doe",
            expiryMonth = 6,
            expiryYear = DateTime.UtcNow.Year + 3,
            creditLimit
        });
        response.EnsureSuccessStatusCode();

        return (await ReadJsonAsync(response)).GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>();
}
