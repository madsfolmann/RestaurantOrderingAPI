using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Common.Models;

namespace RestaurantOrderingAPI.Features.Orders;

public class OrderDTO
{
    public class Response
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public bool IsTakeAway { get; set; }
        public DateTime? PickUpAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public OrderStatus StatusId { get; set; }
        public string StatusName { get; set; }
        public bool CanBeCancelled { get; set; }
        public bool CanBeEdited { get; set; }
        public int BasketId { get; set; }
        public string UserId { get; set; }
    }

    public class Query : BaseQuery
    {
        public Query() : base(50) { }
        public string? UserId { get; set; }
        public HashSet<OrderStatus>? OrderStatuses { get; set; }
    }

    public class CreateRequest
    {
        public int BasketId { get; set; }
        public bool IsTakeAway { get; set; } = false;
        public DateTime? PickUpAt { get; set; }
        public string? Comment { get; set; }
    }

    public class UpdateRequest
    {
        public int Id { get; set; }
        public bool IsTakeAway { get; set; }
        public DateTime? PickUpAt { get; set; }
        public string? Comment { get; set; }
    }
}
