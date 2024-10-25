namespace RestaurantOrderingAPI.Features.Ingredients;

public interface IIngredientService
{
    public Task<int> CreateAsync(IngredientDTO.CreateRequest req);
    public Task UpdateAsync(IngredientDTO.UpdateRequest req);
    public Task ChangeStockAsync(int id, bool inStock);
    public Task DeleteAsync(int id);
    public Task AddRemoveAllergenAsync(int id, IngredientDTO.AddRemoveAllergenRequest req);
    public Task<List<IngredientDTO.Response>> GetAllAsync(CancellationToken ctoken = default);
    public Task<List<IngredientDTO.AllergenResponse>> GetAllergensAsync(
        HashSet<int> ingredientIds, CancellationToken ctoken = default);
}
