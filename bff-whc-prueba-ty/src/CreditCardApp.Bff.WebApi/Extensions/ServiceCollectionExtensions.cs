using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.RateLimiting;
using CreditCardApp.Bff.WebApi.Clients;
using CreditCardApp.Bff.WebApi.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;

namespace CreditCardApp.Bff.WebApi.Extensions;

internal static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "Frontend";

    public static IServiceCollection AddCoreApiClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CoreApiOptions>()
            .Bind(configuration.GetSection(CoreApiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthorizationHandler>();

        services.AddCoreApiClient<AuthApiClient>(options => options.AuthenticationBaseUrl);
        services.AddCoreApiClient<CardsApiClient>(options => options.CardsBaseUrl);
        services.AddCoreApiClient<PaymentsApiClient>(options => options.PaymentsBaseUrl);

        return services;
    }

    public static IServiceCollection AddBffSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection().SetApplicationName("CreditCardApp.Bff");

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = SessionTicket.CookieName;
                options.Cookie.Path = "/api";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.SlidingExpiration = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();

        services.AddOptions<LoginRateLimitOptions>()
            .Bind(configuration.GetSection(LoginRateLimitOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, _) => new ValueTask(
                TypedResults.Problem(
                    detail: "Demasiados intentos de inicio de sesión. Espera un momento e inténtalo nuevamente.",
                    statusCode: StatusCodes.Status429TooManyRequests,
                    title: "Demasiadas solicitudes")
                .ExecuteAsync(context.HttpContext));

            options.AddPolicy(LoginRateLimitOptions.PolicyName, httpContext =>
            {
                var settings = httpContext.RequestServices.GetRequiredService<IOptions<LoginRateLimitOptions>>().Value;

                return RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }

    public static IServiceCollection AddBffCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        return services;
    }

    public static IServiceCollection AddBffHttp(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));

        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHealthChecks();

        return services;
    }

    public static IServiceCollection AddBffApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CreditCardApp BFF",
                Version = "v1",
                Description = "Backend For Frontend de la aplicación web. La sesión viaja en una cookie HttpOnly que se crea con POST /api/v1/auth/login."
            });

            var csrfScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = CsrfHeaderMiddleware.HeaderName,
                In = ParameterLocation.Header,
                Description = $"Escribe {CsrfHeaderMiddleware.HeaderValue}. Todas las rutas /api exigen el header {CsrfHeaderMiddleware.HeaderName}.",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = CsrfHeaderMiddleware.HeaderName }
            };

            options.AddSecurityDefinition(CsrfHeaderMiddleware.HeaderName, csrfScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [csrfScheme] = [] });
        });

        return services;
    }

    private static void AddCoreApiClient<TClient>(this IServiceCollection services, Func<CoreApiOptions, Uri> baseUrl)
        where TClient : class
    {
        services.AddHttpClient<TClient>((serviceProvider, client) =>
                client.BaseAddress = baseUrl(serviceProvider.GetRequiredService<IOptions<CoreApiOptions>>().Value))
            .AddHttpMessageHandler<ForwardAuthorizationHandler>()
            .AddStandardResilienceHandler(options =>
            {
                var defaultShouldHandle = options.Retry.ShouldHandle;
                options.Retry.ShouldHandle = args =>
                    IsSafeMethod(args.Context.GetRequestMessage()?.Method)
                        ? defaultShouldHandle(args)
                        : ValueTask.FromResult(false);
            });
    }

    private static bool IsSafeMethod(HttpMethod? method) =>
        method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Options;
}
