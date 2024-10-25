namespace RestaurantOrderingAPI.Application.Common.Constants;

public enum OrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Processing = 3,
    ReadyForPickup = 4,
    Done = 5,
    Cancelled = 6,
}