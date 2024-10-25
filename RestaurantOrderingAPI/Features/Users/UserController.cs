using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Features.Auth;

namespace RestaurantOrderingAPI.Features.Users;

[ApiController] //AutoValidates ModelState for each method
[Route("api/users")]
[Authorize]
public class UserController(
    IUserService userService,
    IAuthorizationService authorizationService,
    IAuthService authService
    ) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IAuthService _authService = authService;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private async Task<bool> CanManageUsersAsync()
        => (await _authorizationService.AuthorizeAsync(User, Policy.CanManageUsers)).Succeeded;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer(UserDTO.CreateRequest req)
        => Ok(await _userService.CreateAsync(req, Role.Customer));

    [HttpPost("register/employee/{role}")]
    [Authorize(Policy = Policy.CanManageUsers)]
    public async Task<IActionResult> RegisterUser(string role, UserDTO.CreateRequest req)
    {
        if (role == Role.Admin)
        {
            bool isSuperUser = User.IsInRole(Role.SuperUser);
            if (!isSuperUser) return Forbid();

            return Ok(await _userService.CreateAsync(req, Role.Admin));
        }
        else if (role == Role.Chef || role == Role.Staff)
        {
            return Ok(await _userService.CreateAsync(req, role));
        }

        return BadRequest("Invalid role specified.");
    }

    [HttpGet]
    [Authorize(Policy = Policy.CanReadUsers)]
    public async Task<IActionResult> Query([FromQuery] UserDTO.Query q, CancellationToken ctoken)
    {
        return Ok(await _userService.QueryAsync(q, ctoken));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserDTO.UpdateRequest req)
    {
        if (User.Id() != req.Id) return Forbid();

        await _userService.UpdateAsync(req);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(string id)
    {
        var roles = await _userService.GetRoles(id);

        bool selfDelete = User.Id() == id;
        if (!selfDelete)
        {
            if (!await CanManageUsersAsync()) return Forbid();

            if (roles.Contains(Role.Admin) && !User.IsInRole(Role.SuperUser)) return Forbid();
        }

        await _userService.SoftDeleteAsync(id);
        await _authService.LogoutUserEverywhere(id);

        return Ok();
    }
}
