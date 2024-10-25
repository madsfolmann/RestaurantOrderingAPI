using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Application.Filters;

namespace RestaurantOrderingAPI.Features.Users;

public class UserService(
    AppDbContext context,
    UserManager<User> userManager,
    ILogger<UserService> logger
    ) : IUserService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<string> CreateAsync(UserDTO.CreateRequest req, string role)
    {
        var user = new User
        {
            Email = req.Email,
            UserName = req.Email,
            Name = req.Name,
            Age = req.Age,
            IsActive = true,
        };

        var isEmailTaken = await _context.Users.AnyAsync(x => x.Email == req.Email);
        if (isEmailTaken) throw new BadRequestExcept("User already exist!");

        var createUserResult = await _userManager.CreateAsync(user, req.Password);
        if (!createUserResult.Succeeded)
            throw new InternalServerErrorExcept(string.Join(", ", createUserResult.Errors.Select(e => e.Description)));

        var addRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (!addRoleResult.Succeeded)
            throw new InternalServerErrorExcept(string.Join(", ", createUserResult.Errors.Select(e => e.Description)));

        return user.Id;
    }

    public async Task<List<string>> GetRoles(string userId, CancellationToken ctoken)
    {
        var roles = await _context.UserRoles.AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ctoken);

        if (roles.Count == 0)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(u => u.Id == userId, ctoken);
            if (!userExists) throw new NotFoundExcept("User not found!");
            else return [];
        }

        return roles;
    }

    public async Task<List<UserDTO.Response>> QueryAsync(
        UserDTO.Query q, CancellationToken ctoken)
    {
        var l = _context.Users.AsNoTracking().Select(u => new UserDTO.Response
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name,
            Age = u.Age,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
        }).AsQueryable();

        if (q.IsActive != null)
            l = l.Where(u => u.IsActive == q.IsActive);

        if (q.Search != null)
            l = l.Where(u =>
                (u.Name != null && u.Name.Contains(q.Search)) ||
                (u.Email != null && u.Email.Contains(q.Search)));

        bool asc = q.Ascending;
        l = q.SortBy switch
        {
            nameof(User.Name) => l.SortByDir(u => u.Name, asc),
            nameof(User.Age) => l.SortByDir(u => u.Age, asc),
            _ => l.SortByDir(u => u.CreatedAt, asc)
        };

        l = l.Paginate(q.PageNumber, q.PageSize);

        return await l.ToListAsync(ctoken);
    }

    public async Task SoftDeleteAsync(string id)
    {
        int count = await _context.Users
            .Where(u => u.Id == id && u.IsActive)
            .ExecuteUpdateAsync(u =>
                u.SetProperty(u => u.IsActive, false)
            );

        if (count == 0) throw new NotFoundExcept("No active user found!");

        _logger.LogInformation("Deleted user {UserId}.", id);
    }

    public async Task UpdateAsync(UserDTO.UpdateRequest req)
    {
        var user = await _context.Users
            .FindAsync(req.Id)
            ?? throw new NotFoundExcept("User not found!");

        user.Age = req.Age;
        user.Name = req.Name;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
