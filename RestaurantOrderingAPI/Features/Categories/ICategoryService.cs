namespace RestaurantOrderingAPI.Features.Categories;

public interface ICategoryService
{
    public Task<List<CategoryDTO.Response>> GetAllAsync(CancellationToken ctoken = default);
    public Task<int> CreateAsync(CategoryDTO.CreateRequest req);
    public Task UpdateAsync(CategoryDTO.UpdateRequest req);
    public Task DeleteAsync(int id);
}
