namespace RestaurantOrderingAPI.Features.Orders;

public interface IOrderService
{
    public Task<int> CreateAsync(OrderDTO.CreateRequest req, string userId);
    public Task UpdateAsync(OrderDTO.UpdateRequest req);
    public Task CancelAsync(int id);
    public Task ConfirmAsync(int id);
    public Task MarkProcessingAsync(int id);
    public Task MarkReadyForPickupAsync(int id);
    public Task MarkPickedUpAsync(int id);
    public Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken = default);
    public Task<OrderDTO.Response> GetByIdAsync(int id, CancellationToken ctoken = default);
    public Task<OrderDTO.Response> GetDraftByUserIdAsync(
        string userId, CancellationToken ctoken = default);
    public Task<List<OrderDTO.Response>> QueryAsync(
        OrderDTO.Query req, CancellationToken ctoken = default);
}
