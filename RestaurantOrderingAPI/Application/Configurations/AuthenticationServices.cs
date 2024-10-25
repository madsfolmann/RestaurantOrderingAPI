using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Features.Tokens;

namespace RestaurantOrderingAPI.Application.Configurations;
public static class AuthenticationServices
{
    public static IServiceCollection AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
        var signingKey = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);
        services.Configure<JwtSettings>(jwtSettingsSection);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.MapInboundClaims = false;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(0),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(signingKey),
                RoleClaimType = ClaimType.Role,
            };
        });

        return services;
    }
}