namespace RestaurantOrderingAPI.Features.Baskets;

public interface IBasketService
{
    public Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken = default);
    public Task<int> CreateAsync(string userId);
    public Task<int> GetOrCreateAsync(string userId, CancellationToken ctoken = default);
    public Task<BasketDTO.Response> GetByIdAsync(int id, CancellationToken ctoken = default);
    public Task DeleteAsync(int id);
    public Task<decimal> CalcTotalPrice(int id, CancellationToken ctoken = default);
}
