namespace RestaurantOrderingAPI.Features.Users;

public interface IUserService
{
    public Task<string> CreateAsync(UserDTO.CreateRequest req, string role);
    public Task UpdateAsync(UserDTO.UpdateRequest req);
    public Task SoftDeleteAsync(string id);
    public Task<List<UserDTO.Response>> QueryAsync(
        UserDTO.Query q, CancellationToken ctoken = default);
    public Task<List<string>> GetRoles(string userId, CancellationToken ctoken = default);
}
