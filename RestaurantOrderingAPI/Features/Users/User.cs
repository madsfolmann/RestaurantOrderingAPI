using Microsoft.AspNetCore.Identity;

namespace RestaurantOrderingAPI.Features.Users;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
