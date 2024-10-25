using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Extensions;

namespace RestaurantOrderingAPI.Features.Baskets;

[Route("api/baskets")]
[ApiController]
[Authorize]
public class BasketController(IBasketService basketService) : ControllerBase
{
    private readonly IBasketService _basketService = basketService;

    [HttpPost]
    public async Task<IActionResult> GetOrCreate(CancellationToken ctoken)
    {
        return Ok(await _basketService.GetOrCreateAsync(User.Id(), ctoken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ctoken)
    {
        if (!await _basketService.IsOwnerAsync(id, User.Id(), ctoken))
            return Forbid();

        return Ok(await _basketService.GetByIdAsync(id, ctoken));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ctoken)
    {
        if (!await _basketService.IsOwnerAsync(id, User.Id(), ctoken))
            return Forbid();

        await _basketService.DeleteAsync(id);
        return Ok();
    }
}
