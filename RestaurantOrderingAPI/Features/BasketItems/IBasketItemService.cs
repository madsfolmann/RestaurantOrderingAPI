namespace RestaurantOrderingAPI.Features.BasketItems;

public interface IBasketItemService
{
    public Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken = default);
    public Task<int> CreateAsync(BasketItemDTO.CreateRequest req);
    public Task UpdateIngredientQuantityAsync(
        int id, BasketItemDTO.UpdateIngredientQuantityRequest req);
    public Task UpdateQuantityAsync(int id, int quantity);
    public Task DeleteAsync(int id);
}