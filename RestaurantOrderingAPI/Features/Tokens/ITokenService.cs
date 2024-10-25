namespace RestaurantOrderingAPI.Features.Tokens;

public interface ITokenService
{
    public Task<TokenPair> CreateTokenPairAsync(string userId, CancellationToken ctoken = default);
    public Task<TokenPair> RefreshTokenAsync(string token, CancellationToken ctoken = default);
}
