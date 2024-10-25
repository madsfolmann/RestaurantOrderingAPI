namespace RestaurantOrderingAPI.Features.Products;

public interface IProductService
{
    public Task<int> CreateAsync(ProductDTO.CreateRequest req);
    public Task DeleteAsync(int id);
    public Task UpdateAsync(ProductDTO.UpdateRequest req);
    public Task AddRemoveIndgredientAsync(int id, ProductDTO.AddRemoveIngredientRequest req);
    public Task ChangeActiveStatusAsync(int id, bool active);
    public Task<ProductDTO.Response> GetByIdAsync(
        int id, bool requireIsActive, CancellationToken ctoken = default);
    public Task<List<ProductDTO.QueryResponse>> QueryAsync(
        ProductDTO.Query q, CancellationToken ctoken = default);
}
