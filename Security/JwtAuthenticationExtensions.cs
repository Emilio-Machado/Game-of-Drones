using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Game_of_Drones.Security;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddGameJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtSecret = jwtSection[nameof(JwtOptions.Secret)];

        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            if (!environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "Jwt:Secret must be configured outside the Development environment.");
            }

            jwtSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            configuration[$"{JwtOptions.SectionName}:{nameof(JwtOptions.Secret)}"] = jwtSecret;
        }

        var jwtSigningKey = DecodeSigningKey(jwtSecret);
        var jwtIssuer = jwtSection[nameof(JwtOptions.Issuer)]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAudience = jwtSection[nameof(JwtOptions.Audience)]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        services
            .AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
            .Validate(options => options.LifetimeMinutes is >= 5 and <= 1440,
                "Jwt:LifetimeMinutes must be between 5 and 1440.")
            .ValidateOnStart();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSigningKey),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static byte[] DecodeSigningKey(string secret)
    {
        byte[] signingKey;

        try
        {
            signingKey = Convert.FromBase64String(secret);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("Jwt:Secret must be a valid Base64 value.", exception);
        }

        if (signingKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret must decode to at least 256 bits (32 bytes).");
        }

        return signingKey;
    }
}
