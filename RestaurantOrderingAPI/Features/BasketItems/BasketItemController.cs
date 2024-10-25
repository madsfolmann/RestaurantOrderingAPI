using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Features.Baskets;

namespace RestaurantOrderingAPI.Features.BasketItems;

[Route("api/baskets/items")]
[ApiController]
[Authorize]
public class BasketItemController(IBasketItemService basketItemService, IBasketService basketService) : ControllerBase
{
    private readonly IBasketItemService _basketItemService = basketItemService;
    private readonly IBasketService _basketService = basketService;

    [HttpPost]
    public async Task<IActionResult> Create(BasketItemDTO.CreateRequest req)
    {
        if (!await _basketService.IsOwnerAsync(req.BasketId, User.Id()))
            return Forbid();

        return Ok(await _basketItemService.CreateAsync(req));
    }

    [HttpPost("{id}/ingredients/quantity")]
    public async Task<IActionResult> UpdateIngredientQuantity(
        int id, BasketItemDTO.UpdateIngredientQuantityRequest req)
    {
        if (!await _basketItemService.IsOwnerAsync(id, User.Id()))
            return Forbid();

        await _basketItemService.UpdateIngredientQuantityAsync(id, req);
        return Ok();
    }

    [HttpPost("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        if (!await _basketItemService.IsOwnerAsync(id, User.Id()))
            return Forbid();

        await _basketItemService.UpdateQuantityAsync(id, quantity);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _basketItemService.IsOwnerAsync(id, User.Id()))
            return Forbid();

        await _basketItemService.DeleteAsync(id);
        return Ok();
    }
}
