using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Features.Tokens;

namespace RestaurantOrderingAPI.Features.Auth;
[Route("api/auth")]
[ApiController]
[Authorize]
public class AuthController(
    IAuthService authService,
    ITokenService tokenService
) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AuthDTO.LoginRequest req, CancellationToken ctoken)
        => Ok(await _authService.LoginAsync(req, ctoken));

    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutEverywhere()
    {
        await _authService.LogoutUserEverywhere(User.Id());
        return Ok();
    }

    [HttpPost("token/refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(string token, CancellationToken ctoken)
        => Ok(await _tokenService.RefreshTokenAsync(token, ctoken));

    [HttpPost("token/validate")]
    public IActionResult ValidateAccessToken() => Ok();
}