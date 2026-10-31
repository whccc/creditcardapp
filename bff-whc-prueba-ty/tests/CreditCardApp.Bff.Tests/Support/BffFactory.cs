using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace CreditCardApp.Bff.Tests.Support;

public sealed class BffFactory : WebApplicationFactory<Program>
{
    private const string CoreBaseUrl = "http://core.test";

    public StubCoreApi CoreApi { get; } = new();

    public HttpClient CreateBrowserClient(bool withCsrfHeader = true)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });

        if (withCsrfHeader)
        {
            client.DefaultRequestHeaders.Add("X-CSRF", "1");
        }

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("CoreApi:AuthenticationBaseUrl", CoreBaseUrl);
        builder.UseSetting("CoreApi:CardsBaseUrl", CoreBaseUrl);
        builder.UseSetting("CoreApi:PaymentsBaseUrl", CoreBaseUrl);
        builder.UseSetting("RateLimiting:Login:PermitLimit", "100");

        builder.ConfigureTestServices(services =>
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
                options.HttpMessageHandlerBuilderActions.Add(handlerBuilder => handlerBuilder.PrimaryHandler = CoreApi)));
    }
}
