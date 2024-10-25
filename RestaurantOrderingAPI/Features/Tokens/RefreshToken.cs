using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Features.Tokens;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime ExpirationDate { get; set; }
}
