using RestaurantOrderingAPI.Application.Common;
using RestaurantOrderingAPI.Features.BasketItems;
using RestaurantOrderingAPI.Features.Orders;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Features.Baskets;

public class Basket
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public Order? Order { get; set; }
    public List<BasketItem> BasketItems { get; set; } = [];
}
