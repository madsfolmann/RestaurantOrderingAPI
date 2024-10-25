using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Features.Baskets;

namespace RestaurantOrderingAPI.Features.Orders;

public class OrderService(AppDbContext context, IBasketService basketService) : IOrderService
{
    private readonly AppDbContext _context = context;
    private readonly IBasketService _basketService = basketService;
    private readonly DbSet<Order> _orders = context.Set<Order>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task CancelAsync(int id)
    {
        await RequireStatus(OrderStatus.Confirmed, id);

        DateTime currentDateTime = DateTime.UtcNow;
        await _orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.StatusId, OrderStatus.Cancelled)
                .SetProperty(o => o.CancelledAt, currentDateTime)
                .SetProperty(o => o.UpdatedAt, currentDateTime)
            );
    }

    private async Task MakePaymentAsync(int orderId)
    {
        var order = _orders
            .Select(o => new
            {
                o.Id,
                o.BasketId
            })
            .FirstOrDefault(o => o.Id == orderId)
            ?? throw new NotFoundExcept("Order not found!");

        var totalAmount = await _basketService.CalcTotalPrice(order.BasketId);

        //Make real life payment here
    }

    public async Task ConfirmAsync(int id)
    {
        await RequireStatus(OrderStatus.Draft, id);

        var nonStockIngredients = await _context.Orders
            .Where(b => b.Id == id)
            .SelectMany(b => b.Basket.BasketItems.SelectMany(bi =>
                bi.Product.Ingredients.Union(bi.ExtraIngredients)
                    .Where(i => !i.InStock)
                    .Select(i => new { i.Name })
            ))
            .ToListAsync();

        if (nonStockIngredients.Count != 0)
            throw new BadRequestExcept($"Missing ingredients: {string.Join(", ", nonStockIngredients.Select(x => x.Name))}");

        //Pay
        await MakePaymentAsync(id);

        //Confirm order
        DateTime currentDateTime = DateTime.UtcNow;
        await _orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.StatusId, OrderStatus.Confirmed)
                .SetProperty(o => o.ConfirmedAt, currentDateTime)
                .SetProperty(o => o.UpdatedAt, currentDateTime)
            );
    }

    public async Task<int> CreateAsync(OrderDTO.CreateRequest req, string userId)
    {
        if (await _orders.AnyAsync(o => o.BasketId == req.BasketId))
            throw new BadRequestExcept("Basket belongs to another order!");

        var order = new Order
        {
            BasketId = req.BasketId,
            IsTakeAway = req.IsTakeAway,
            PickUpAt = req.PickUpAt,
            Comment = req.Comment,
            UserId = userId,
        };

        if (order.PickUpAt == null)
            order.PickUpAt = DateTime.UtcNow.AddMinutes(5);
        else if (order.PickUpAt < DateTime.UtcNow.AddMinutes(5) || order.PickUpAt > DateTime.UtcNow.AddDays(2))
            throw new BadRequestExcept("PickUpAt must be between 5 minutes from now and 2 days forward.");

        _orders.Add(order);
        await SaveAsync();
        return order.Id;
    }

    public async Task<OrderDTO.Response> GetByIdAsync(int id, CancellationToken ctoken)
    {
        var order = await _orders.Select(o => new OrderDTO.Response
        {
            Id = o.Id,
            Comment = o.Comment,
            IsTakeAway = o.IsTakeAway,
            PickUpAt = o.PickUpAt,
            CreatedAt = o.CreatedAt,
            ConfirmedAt = o.ConfirmedAt,
            CancelledAt = o.CancelledAt,
            StatusId = o.StatusId,
            StatusName = OrderStatus.GetName(o.StatusId),
            CanBeCancelled = o.StatusId == OrderStatus.Confirmed,
            CanBeEdited = o.StatusId == OrderStatus.Draft,
            BasketId = o.BasketId,
            UserId = o.UserId,
        }).FirstOrDefaultAsync(o => o.Id == id, ctoken)
            ?? throw new NotFoundExcept("Order not found!");

        return order;
    }

    public async Task<List<OrderDTO.Response>> QueryAsync(OrderDTO.Query q, CancellationToken ctoken)
    {
        var l = _orders.AsNoTracking()
            .Select(o => new OrderDTO.Response
            {
                Id = o.Id,
                Comment = o.Comment,
                IsTakeAway = o.IsTakeAway,
                PickUpAt = o.PickUpAt,
                CreatedAt = o.CreatedAt,
                ConfirmedAt = o.ConfirmedAt,
                CancelledAt = o.CancelledAt,
                StatusId = o.StatusId,
                StatusName = OrderStatus.GetName(o.StatusId),
                CanBeCancelled = o.StatusId == OrderStatus.Confirmed,
                CanBeEdited = o.StatusId == OrderStatus.Draft,
                BasketId = o.BasketId,
                UserId = o.UserId,
            })
            .AsQueryable();

        bool asc = q.Ascending;
        l = q.SortBy switch
        {
            nameof(Order.ConfirmedAt) => l.SortByDir(x => x.ConfirmedAt, asc),
            nameof(Order.PickUpAt) => l.SortByDir(x => x.PickUpAt, asc),
            _ => l.SortByDir(x => x.CreatedAt, asc)
        };

        if (q.UserId != null)
            l = l.Where(o => o.UserId == q.UserId);

        if (q.OrderStatuses != null)
            l = l.Where(o => q.OrderStatuses.Contains(o.StatusId));

        l = l.Paginate(q.PageNumber, q.PageSize);

        return await l.ToListAsync(ctoken);
    }

    public async Task UpdateAsync(OrderDTO.UpdateRequest req)
    {
        await RequireStatus(OrderStatus.Draft, req.Id);

        var order = await _orders.FindAsync(req.Id) ?? throw new NotFoundExcept("Order not found!");

        order.IsTakeAway = req.IsTakeAway;
        order.PickUpAt = req.PickUpAt;
        order.Comment = req.Comment;

        if (order.PickUpAt == null)
            order.PickUpAt = DateTime.UtcNow.AddMinutes(5);
        else if (order.PickUpAt < DateTime.UtcNow.AddMinutes(5) || order.PickUpAt > DateTime.UtcNow.AddDays(2))
            throw new BadRequestExcept("PickUpAt must be between 5 minutes from now and 2 days forward.");

        order.UpdatedAt = DateTime.UtcNow;

        await SaveAsync();
    }

    public async Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken)
    {
        return await _orders
            .AnyAsync(o => o.Id == id && o.UserId == userId, ctoken);
    }

    public async Task<OrderDTO.Response> GetDraftByUserIdAsync(string userId, CancellationToken ctoken)
    {
        var order = await _orders
            .Where(o => o.UserId == userId && o.StatusId == OrderStatus.Draft)
            .Select(o => new { o.Id })
            .FirstOrDefaultAsync(ctoken)
            ?? throw new NotFoundExcept("No order found!");

        return await GetByIdAsync(order.Id, ctoken);
    }

    public async Task MarkProcessingAsync(int id)
    {
        await RequireStatus(OrderStatus.Confirmed, id);

        DateTime currentDateTime = DateTime.UtcNow;
        await _orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.StatusId, OrderStatus.Processing)
                .SetProperty(o => o.UpdatedAt, currentDateTime)
            );
    }

    public async Task MarkReadyForPickupAsync(int id)
    {
        await RequireStatus(OrderStatus.Processing, id);

        DateTime currentDateTime = DateTime.UtcNow;
        await _orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.StatusId, OrderStatus.ReadyForPickup)
                .SetProperty(o => o.UpdatedAt, currentDateTime)
            );
    }

    public async Task MarkPickedUpAsync(int id)
    {
        await RequireStatus(OrderStatus.ReadyForPickup, id);

        DateTime currentDateTime = DateTime.UtcNow;
        await _orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.StatusId, OrderStatus.Done)
                .SetProperty(o => o.UpdatedAt, currentDateTime)
            );
    }

    private async Task RequireStatus(OrderStatus status, int orderId)
    {
        var order = await _orders
            .Where(o => o.Id == orderId)
            .Select(o => new
            {
                o.StatusId,
            })
            .FirstOrDefaultAsync()
            ?? throw new BadRequestExcept("Order not found!");


        if (order.StatusId != status)
            throw new BadRequestExcept($"Order is not in state {status.GetDisplayName()}!");
    }
}
