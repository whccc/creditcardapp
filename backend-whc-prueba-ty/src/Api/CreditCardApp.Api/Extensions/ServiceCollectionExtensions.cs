using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using CreditCardApp.Api.Http;
using CreditCardApp.Api.Integration;
using CreditCardApp.Api.Seeding;
using CreditCardApp.Modules.Authentication.Infrastructure;
using CreditCardApp.Modules.Authentication.Infrastructure.Security;
using CreditCardApp.Modules.Cards.Infrastructure;
using CreditCardApp.Modules.Payments.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CreditCardApp.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddAuthenticationModule(configuration)
            .AddCardsModule(configuration)
            .AddPaymentsModule<CardsChargeGateway>(configuration);

    public static IServiceCollection AddDemoDataSeeding(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DemoDataOptions>().Bind(configuration.GetSection(DemoDataOptions.SectionName));
        services.AddScoped<DemoDataSeeder>();

        return services;
    }

    public static IServiceCollection AddApiSecurity(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Name
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApiHttp(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));

        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHealthChecks();

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CreditCardApp Core API",
                Version = "v1",
                Description = "API interna del monolito modular. La consume el BFF; no está pensada para el navegador."
            });

            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Pega el accessToken obtenido en POST /api/v1/auth/login.",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [bearerScheme] = [] });
        });

        return services;
    }
}
