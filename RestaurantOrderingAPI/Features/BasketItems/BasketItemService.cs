using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Application.JoiningTables;

namespace RestaurantOrderingAPI.Features.BasketItems;

public class BasketItemService(AppDbContext context) : IBasketItemService
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<BasketItem> _basketItem = context.Set<BasketItem>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task UpdateIngredientQuantityAsync(int id, BasketItemDTO.UpdateIngredientQuantityRequest req)
    {
        bool removeIngredient = req.Quantity <= 0;

        if (removeIngredient)
        {
            var count = await _context.BasketItemIngredients
                .Where(x => x.BasketItemId == id && x.IngredientId == req.IngredientId)
                .ExecuteDeleteAsync();

            if (count == 0)
                throw new BadRequestExcept("Ingredient not added!");

            return;
        }

        var exists = await _context.BasketItems.Where(bi => bi.Id == id)
            .Select(bi => new
            {
                Ingredient = _context.Ingredients.Any(i => i.Id == req.IngredientId),
                Pair = _context.BasketItemIngredients.Any(x => x.BasketItemId == id && x.IngredientId == req.IngredientId)
            }).FirstOrDefaultAsync()
            ?? throw new NotFoundExcept("BasketItem not found!");

        if (!exists.Ingredient) throw new NotFoundExcept("Ingredient not found!");

        if (exists.Pair)
            await _context.BasketItemIngredients
            .Where(x => x.BasketItemId == id && x.IngredientId == req.IngredientId)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(bi => bi.Quantity, req.Quantity)
            );
        else
        {
            var pair = new BasketItemIngredient
            {
                BasketItemId = id,
                IngredientId = req.IngredientId,
                Quantity = req.Quantity
            };

            _context.BasketItemIngredients.Add(pair);
            await SaveAsync();
        }
    }

    public async Task<int> CreateAsync(BasketItemDTO.CreateRequest req)
    {
        var exists = await _context.Products.Where(p => p.Id == req.ProductId && p.IsActive)
            .Select(x => new
            {
                Basket = _context.Baskets.Any(b => b.Id == req.BasketId),
            }).FirstOrDefaultAsync()
            ?? throw new NotFoundExcept("Product not found!");

        if (!exists.Basket) throw new NotFoundExcept("Basket not found!");

        var basketItem = new BasketItem
        {
            ProductId = req.ProductId,
            BasketId = req.BasketId,
            Quantity = req.Quantity
        };

        _context.Add(basketItem);
        await SaveAsync();
        return basketItem.Id;
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _basketItem.AnyAsync(bi => bi.Id == id))
            throw new NotFoundExcept("BasketItem not found!");

        await _basketItem.Where(bi => bi.Id == id).ExecuteDeleteAsync();
    }

    public async Task UpdateQuantityAsync(int id, int quantity)
    {
        if (quantity <= 0)
        {
            await DeleteAsync(id);
            return;
        }

        if (!await _basketItem.AnyAsync(bi => bi.Id == id))
            throw new NotFoundExcept("BasketItem not found!");

        await _basketItem
            .Where(bi => bi.Id == id)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(bi => bi.Quantity, quantity)
            );
    }

    public async Task<bool> IsOwnerAsync(int id, string userId, CancellationToken ctoken)
    {
        return await _basketItem
            .AnyAsync(bi => bi.Id == id && bi.Basket.UserId == userId, ctoken);
    }
}