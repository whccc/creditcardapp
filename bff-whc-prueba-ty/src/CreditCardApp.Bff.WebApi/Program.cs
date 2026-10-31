using CreditCardApp.Bff.WebApi.Extensions;
using CreditCardApp.Bff.WebApi.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCoreApiClients(builder.Configuration)
    .AddBffSecurity(builder.Configuration)
    .AddBffCors(builder.Configuration)
    .AddBffHttp()
    .AddBffApiDocumentation();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.DocumentTitle = "CreditCardApp BFF");
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);
app.UseMiddleware<CsrfHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
