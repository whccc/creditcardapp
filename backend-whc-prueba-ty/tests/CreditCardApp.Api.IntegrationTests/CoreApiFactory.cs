using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace CreditCardApp.Api.IntegrationTests;

public sealed class CoreApiFactory : WebApplicationFactory<Program>
{
    public const string DemoEmail = "demo@creditcard.app";
    public const string DemoPassword = "Demo1234!";

    private readonly string _dataDirectory =
        Path.Combine(Path.GetTempPath(), "creditcardapp-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:Authentication", $"Data Source={Path.Combine(_dataDirectory, "authentication.db")}");
        builder.UseSetting("ConnectionStrings:Cards", $"Data Source={Path.Combine(_dataDirectory, "cards.db")}");
        builder.UseSetting("ConnectionStrings:Payments", $"Data Source={Path.Combine(_dataDirectory, "payments.db")}");
        builder.UseSetting("Jwt:SigningKey", "integration-tests-signing-key-0123456789abcdef");
        builder.UseSetting("DemoData:Enabled", "true");
        builder.UseSetting("DemoData:Email", DemoEmail);
        builder.UseSetting("DemoData:Password", DemoPassword);
        builder.UseSetting("DemoData:FullName", "Usuario Demo");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        SqliteConnection.ClearAllPools();

        try
        {
            Directory.Delete(_dataDirectory, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}
