using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Features.Tokens;

public class TokenService(
    AppDbContext context,
    IOptions<JwtSettings> jwtSettings,
    IUserService userService
    ) : ITokenService
{
    private readonly AppDbContext _context = context;
    private readonly IUserService _userService = userService;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    private string GenerateAccessToken(List<Claim> claims)
    {
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            SecurityAlgorithms.HmacSha512Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(string userId)
        => new()
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            ExpirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes),
        };

    private async Task<List<Claim>> CreateClaims(string userId, CancellationToken ctoken)
    {
        var roles = await _userService.GetRoles(userId, ctoken);

        var claims = new List<Claim>
            {
                new(ClaimType.UserId, userId)
            };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimType.Role, role));
        }

        return claims;
    }

    public async Task<TokenPair> CreateTokenPairAsync(string userId, CancellationToken ctoken)
    {
        var newRefreshToken = GenerateRefreshToken(userId);

        await _context.RefreshTokens.AddAsync(newRefreshToken, ctoken);
        await _context.SaveChangesAsync(ctoken);

        return new TokenPair
        {
            AccessToken = GenerateAccessToken(await CreateClaims(userId, ctoken)),
            RefreshToken = newRefreshToken.Token,
        };
    }

    public async Task<TokenPair> RefreshTokenAsync(string token, CancellationToken ctoken)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, ctoken)
            ?? throw new NotFoundExcept("Refresh token not found!");

        if (refreshToken.ExpirationDate < DateTime.UtcNow)
            throw new UnauthorizedAccessExcept("Refresh token too old!");

        string userId = refreshToken.UserId;

        var newRefreshToken = GenerateRefreshToken(userId);
        refreshToken.Token = newRefreshToken.Token;
        refreshToken.ExpirationDate = newRefreshToken.ExpirationDate;
        await _context.SaveChangesAsync(ctoken);

        return new TokenPair
        {
            AccessToken = GenerateAccessToken(await CreateClaims(userId, ctoken)),
            RefreshToken = newRefreshToken.Token,
        };
    }
}
