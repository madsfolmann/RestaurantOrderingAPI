using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Features.Tokens;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Features.Auth;

public class AuthService(
    ITokenService tokenService,
    SignInManager<User> signInManager,
    AppDbContext context
) : IAuthService
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly AppDbContext _context = context;

    public async Task<TokenPair> LoginAsync(AuthDTO.LoginRequest req, CancellationToken ctoken)
    {
        var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessExcept($"Credentials is wrong!");
        }

        var user = await _context.Users
            .Where(u => u.Email == req.Email)
            .Select(u => new
            {
                u.Id,
                u.IsActive
            })
            .FirstOrDefaultAsync(ctoken)
            ?? throw new NotFoundExcept("User not found!");
        if (!user.IsActive) throw new BadRequestExcept("User not active!");

        return await _tokenService.CreateTokenPairAsync(user.Id, ctoken);
    }

    public async Task LogoutUserEverywhere(string userId)
    {
        await _context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
