using CreditCardApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ResolveSqliteDataSources();

builder.Services
    .AddApplicationModules(builder.Configuration)
    .AddDemoDataSeeding(builder.Configuration)
    .AddApiSecurity()
    .AddApiHttp()
    .AddApiDocumentation();

var app = builder.Build();

await app.InitializeModulesAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.DocumentTitle = "CreditCardApp Core API");
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
