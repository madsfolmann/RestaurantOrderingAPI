using RestaurantOrderingAPI.Features.Tokens;

namespace RestaurantOrderingAPI.Features.Auth;

public interface IAuthService
{
    public Task<TokenPair> LoginAsync(AuthDTO.LoginRequest req, CancellationToken ctoken = default);
    public Task LogoutUserEverywhere(string userId);
}
