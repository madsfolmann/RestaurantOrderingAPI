using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Features.Ingredients;

[Route("api/ingredients")]
[ApiController]
[Authorize(Policy = Policy.CanManageFood)]
public class IngredientController(IIngredientService ingredientService) : ControllerBase
{
    private readonly IIngredientService _ingredientService = ingredientService;

    [HttpGet("allergens")]
    public async Task<IActionResult> GetAllergens(
        [FromQuery] HashSet<int> ingredientIds, CancellationToken ctoken)
    {
        return Ok(await _ingredientService.GetAllergensAsync(ingredientIds, ctoken));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ctoken)
        => Ok(await _ingredientService.GetAllAsync(ctoken));

    [HttpPost("{id}/stock")]
    public async Task<IActionResult> ChangeStock(int id, bool inStock)
    {
        await _ingredientService.ChangeStockAsync(id, inStock);
        return Ok();
    }

    [HttpPost("{id}/allergens")]
    public async Task<IActionResult> AddRemoveAllergen(
        int id, IngredientDTO.AddRemoveAllergenRequest req)
    {
        await _ingredientService.AddRemoveAllergenAsync(id, req);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ingredientService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(IngredientDTO.UpdateRequest req)
    {
        await _ingredientService.UpdateAsync(req);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> Create(IngredientDTO.CreateRequest req)
        => Ok(await _ingredientService.CreateAsync(req));
}