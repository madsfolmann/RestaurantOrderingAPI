using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Features.Baskets;

namespace RestaurantOrderingAPI.Features.Orders;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrderController(IBasketService basketService, IOrderService orderService, IAuthorizationService authorizationService) : ControllerBase
{
    private readonly IBasketService _basketService = basketService;
    private readonly IOrderService _orderService = orderService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    private async Task<bool> CanManageOrdersAsync()
        => (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrders)).Succeeded;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ctoken)
    {
        if (await CanManageOrdersAsync() || await _orderService.IsOwnerAsync(id, User.Id(), ctoken))
            return Ok(await _orderService.GetByIdAsync(id, ctoken));
        else return Forbid();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (await CanManageOrdersAsync() || await _orderService.IsOwnerAsync(id, User.Id()))
        {
            await _orderService.CancelAsync(id);
            return Ok();
        }
        else return Forbid();
    }

    [HttpGet]
    [Authorize(Policy = Policy.CanManageOrders)]
    public async Task<IActionResult> Query([FromQuery] OrderDTO.Query req, CancellationToken ctoken)
    {
        return Ok(await _orderService.QueryAsync(req, ctoken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDTO.CreateRequest req)
    {
        string userId = User.Id();

        if (!await _basketService.IsOwnerAsync(req.BasketId, userId))
            return Forbid();

        return Ok(await _orderService.CreateAsync(req, userId));
    }

    [HttpPut]
    public async Task<IActionResult> Update(OrderDTO.UpdateRequest req)
    {
        if (!await _orderService.IsOwnerAsync(req.Id, User.Id()))
            return Forbid();

        await _orderService.UpdateAsync(req);
        return Ok();
    }

    [HttpGet("draft")]
    public async Task<IActionResult> GetDraftByUser(CancellationToken ctoken)
    {
        return Ok(await _orderService.GetDraftByUserIdAsync(User.Id(), ctoken));
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        if (!await _orderService.IsOwnerAsync(id, User.Id()))
            return Forbid();

        await _orderService.ConfirmAsync(id);
        return Ok();
    }

    [HttpGet("customer/history")]
    public async Task<IActionResult> GetUserHistory(
        [FromQuery] bool ascending, int pageNumber, int pageSize, CancellationToken ctoken)
    {
        var limitedQuery = new OrderDTO.Query
        {
            UserId = User.Id(),
            OrderStatuses = [
                OrderStatus.Cancelled,
                OrderStatus.Done
            ],
            SortBy = nameof(Order.ConfirmedAt),
            Ascending = ascending,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return Ok(await _orderService.QueryAsync(limitedQuery, ctoken));
    }

    [HttpGet("customer/current")]
    public async Task<IActionResult> GetCurrentByUser(CancellationToken ctoken)
    {
        var query = new OrderDTO.Query
        {
            UserId = User.Id(),
            OrderStatuses = [
                OrderStatus.Confirmed,
                OrderStatus.Processing,
                OrderStatus.ReadyForPickup,
                OrderStatus.Draft,
            ],
            SortBy = nameof(Order.ConfirmedAt),
            PageSize = 20,
        };

        return Ok(await _orderService.QueryAsync(query, ctoken));
    }

    [HttpGet("kitchen")]
    [Authorize(Policy = Policy.CanManageKitchenOrders)]
    public async Task<IActionResult> GetKitchenOrders(CancellationToken ctoken)
    {
        var query = new OrderDTO.Query
        {
            OrderStatuses = [
                OrderStatus.Confirmed,
                OrderStatus.Processing,
                OrderStatus.ReadyForPickup,
            ],
            SortBy = nameof(Order.PickUpAt),
            PageSize = 50,
        };

        return Ok(await _orderService.QueryAsync(query, ctoken));
    }

    [HttpPost("kitchen/{id}/process")]
    [Authorize(Policy = Policy.CanManageKitchenOrders)]
    public async Task<IActionResult> MarkProcessing(int id)
    {
        await _orderService.MarkProcessingAsync(id);
        return Ok();
    }

    [HttpPost("kitchen/{id}/ready-for-pickup")]
    [Authorize(Policy = Policy.CanManageKitchenOrders)]
    public async Task<IActionResult> ReadyForPickup(int id)
    {
        await _orderService.MarkReadyForPickupAsync(id);
        return Ok();
    }

    [HttpPost("kitchen/{id}/picked-up")]
    [Authorize(Policy = Policy.CanManageKitchenOrders)]
    public async Task<IActionResult> PickedUp(int id)
    {
        await _orderService.MarkPickedUpAsync(id);
        return Ok();
    }
}