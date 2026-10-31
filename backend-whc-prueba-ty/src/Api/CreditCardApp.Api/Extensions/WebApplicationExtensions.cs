using CreditCardApp.Api.Seeding;
using CreditCardApp.SharedKernel.Modules;

namespace CreditCardApp.Api.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task InitializeModulesAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();

        foreach (var initializer in scope.ServiceProvider.GetServices<IModuleInitializer>())
        {
            await initializer.InitializeAsync(cancellationToken);
        }

        await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync(cancellationToken);
    }
}
