using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Features.Products;

[ApiController]
[Route("api/products")]
[Authorize(Policy = Policy.CanManageFood)]
public class ProductController(IProductService productService, IAuthorizationService authorizationService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    private async Task<bool> CanManageFoodAsync()
        => (await _authorizationService.AuthorizeAsync(User, Policy.CanManageFood)).Succeeded;

    [HttpPost]
    public async Task<IActionResult> Create(ProductDTO.CreateRequest req)
        => Ok(await _productService.CreateAsync(req));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("{id}/ingredients")]
    public async Task<IActionResult> AddRemoveIndgredients(int id, ProductDTO.AddRemoveIngredientRequest req)
    {
        await _productService.AddRemoveIndgredientAsync(id, req);
        return Ok();
    }

    [HttpPost("{id}/active")]
    public async Task<IActionResult> ChangeActiveStatus(int id, bool active)
    {
        await _productService.ChangeActiveStatusAsync(id, active);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(ProductDTO.UpdateRequest req)
    {
        await _productService.UpdateAsync(req);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ctoken)
    {
        bool requireIsActive = !await CanManageFoodAsync();

        return Ok(await _productService
            .GetByIdAsync(id, requireIsActive, ctoken));
    }

    [HttpGet]
    public async Task<IActionResult> Query(
        [FromQuery] ProductDTO.Query q, CancellationToken ctoken)
    {
        if (!await CanManageFoodAsync())
            q.IsActive = true; //Only show active products to customer

        return Ok(await _productService.QueryAsync(q, ctoken));
    }
}