using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Features.Allergens;

[Route("api/allergens")]
[ApiController]
[Authorize(Policy = Policy.CanManageFood)]
public class AllergenController(IAllergenService allergenService) : ControllerBase
{
    private readonly IAllergenService _allergenService = allergenService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ctoken)
        => Ok(await _allergenService.GetAllAsync(ctoken));

    [HttpPost]
    public async Task<IActionResult> Create(AllergenDTO.CreateRequest req)
        => Ok(await _allergenService.CreateAsync(req));

    [HttpPut]
    public async Task<IActionResult> Update(AllergenDTO.UpdateRequest req)
    {
        await _allergenService.UpdateAsync(req);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _allergenService.DeleteAsync(id);
        return Ok();
    }
}
