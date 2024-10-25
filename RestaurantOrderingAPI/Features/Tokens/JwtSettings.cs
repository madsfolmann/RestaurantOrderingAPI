using System;

namespace RestaurantOrderingAPI.Features.Tokens;

public class JwtSettings
{
    public string SigningKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }
}
