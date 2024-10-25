namespace RestaurantOrderingAPI.Features.Allergens;

public interface IAllergenService
{
    public Task<List<AllergenDTO.Response>> GetAllAsync(CancellationToken ctoken = default);
    public Task<int> CreateAsync(AllergenDTO.CreateRequest req);
    public Task UpdateAsync(AllergenDTO.UpdateRequest req);
    public Task DeleteAsync(int id);
}
