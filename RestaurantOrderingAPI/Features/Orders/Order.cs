using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Features.Baskets;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Features.Orders;

public class Order
{
    public int Id { get; set; }
    public string? Comment { get; set; }
    public bool IsTakeAway { get; set; }
    public DateTime? PickUpAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public OrderStatus StatusId { get; set; } = OrderStatus.Draft;
    public int BasketId { get; set; }
    public Basket Basket { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}
