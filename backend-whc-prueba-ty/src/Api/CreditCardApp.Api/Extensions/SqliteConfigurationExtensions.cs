using Microsoft.Data.Sqlite;

namespace CreditCardApp.Api.Extensions;

internal static class SqliteConfigurationExtensions
{
    private const string InMemoryDataSource = ":memory:";

    public static WebApplicationBuilder ResolveSqliteDataSources(this WebApplicationBuilder builder)
    {
        var resolved = new Dictionary<string, string?>();

        foreach (var entry in builder.Configuration.GetSection("ConnectionStrings").GetChildren())
        {
            if (string.IsNullOrWhiteSpace(entry.Value))
            {
                continue;
            }

            var connection = new SqliteConnectionStringBuilder(entry.Value);
            if (string.IsNullOrWhiteSpace(connection.DataSource) || connection.DataSource == InMemoryDataSource)
            {
                continue;
            }

            var fullPath = Path.GetFullPath(connection.DataSource, builder.Environment.ContentRootPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            connection.DataSource = fullPath;
            resolved[entry.Path] = connection.ToString();
        }

        builder.Configuration.AddInMemoryCollection(resolved);

        return builder;
    }
}
