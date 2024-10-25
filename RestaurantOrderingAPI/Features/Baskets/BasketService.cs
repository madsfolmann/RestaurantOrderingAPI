using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;

namespace RestaurantOrderingAPI.Features.Baskets;

public class BasketService(AppDbContext context) : IBasketService
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<Basket> _baskets = context.Set<Basket>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<int> CreateAsync(string userId)
    {
        var basket = new Basket
        {
            UserId = userId
        };

        _baskets.Add(basket);
        await SaveAsync();
        return basket.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var count = await _baskets.Where(b => b.Id == id).ExecuteDeleteAsync();
        if (count == 0)
            throw new NotFoundExcept("Basket not found!");
    }

    public async Task<BasketDTO.Response> GetByIdAsync(int id, CancellationToken ctoken)
    {
        var basket = await _baskets.Select(b => new BasketDTO.Response
        {
            Id = b.Id,
            Items = b.BasketItems.Select(bi => new BasketDTO.Response.Item
            {
                Id = bi.Id,
                Quantity = bi.Quantity,
                ProductId = bi.ProductId,
                ProductName = bi.Product.Name,
                ExtraIngredients = bi.ExtraIngredients.Select(ei => ei.Name).ToList(),
                TotalPrice = bi.Quantity *
                    (bi.Product.Price + bi.BasketItemIngredients.Sum(x => x.Quantity * x.Ingredient.Price)),
            }).ToList(),
        }).FirstOrDefaultAsync(b => b.Id == id, ctoken)
            ?? throw new NotFoundExcept("Basket not found");

        basket.TotalPrice = basket.Items.Sum(item => item.TotalPrice);

        return basket;
    }

    public async Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken)
    {
        return await _baskets
            .AnyAsync(b => b.Id == id && b.UserId == userId, ctoken);
    }

    public async Task<int> GetOrCreateAsync(string userId, CancellationToken ctoken)
    {
        var basket = await _baskets
            .Where(b =>
                b.UserId == userId
                && (b.Order == null || b.Order.StatusId == OrderStatus.Draft)
            )
            .Select(b => new
            {
                b.Id,
            })
            .FirstOrDefaultAsync(ctoken);

        return basket != null ? basket.Id : await CreateAsync(userId);
    }

    public async Task<decimal> CalcTotalPrice(int id, CancellationToken ctoken)
    {
        var basket = await _baskets
            .Where(b => b.Id == id)
            .Select(b => new
            {
                Items = b.BasketItems.Select(bi => new
                {
                    TotalPrice = bi.Quantity *
                        (bi.Product.Price + bi.BasketItemIngredients.Sum(x => x.Quantity * x.Ingredient.Price)),
                }).ToList(),
            }).FirstOrDefaultAsync(ctoken)
                ?? throw new NotFoundExcept("Basket not found!");

        return basket.Items.Sum(item => item.TotalPrice);
    }
}
