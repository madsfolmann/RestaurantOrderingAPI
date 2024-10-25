using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Features.Categories;

[Route("api/categories")]
[ApiController]
[Authorize(Policy = Policy.CanManageFood)]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ctoken)
        => Ok(await _categoryService.GetAllAsync(ctoken));

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDTO.CreateRequest req)
        => Ok(await _categoryService.CreateAsync(req));

    [HttpPut]
    public async Task<IActionResult> Update(CategoryDTO.UpdateRequest req)
    {
        await _categoryService.UpdateAsync(req);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok();
    }
}