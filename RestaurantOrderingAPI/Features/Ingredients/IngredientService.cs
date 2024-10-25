using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Application.JoiningTables;
using RestaurantOrderingAPI.Features.Allergens;

namespace RestaurantOrderingAPI.Features.Ingredients;

public class IngredientService(AppDbContext context) : IIngredientService
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<Ingredient> _ingredients = context.Set<Ingredient>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<int> CreateAsync(IngredientDTO.CreateRequest req)
    {
        var ingredient = new Ingredient
        {
            Price = req.Price,
            Name = req.Name,
            Description = req.Description,
            InStock = false,
        };

        _context.Add(ingredient);
        await SaveAsync();
        return ingredient.Id;
    }

    public async Task<List<IngredientDTO.AllergenResponse>> GetAllergensAsync(
        HashSet<int> ingredientIds, CancellationToken ctoken)
    {
        var allergens = await _context.IngredientAllergens.AsNoTracking()
            .Where(ia => ingredientIds.Contains(ia.IngredientId))
            .Select(ia => new IngredientDTO.AllergenResponse
            {
                Allergen = new AllergenDTO.Response
                {
                    Id = ia.Allergen.Id,
                    Name = ia.Allergen.Name,
                    Description = ia.Allergen.Description,
                },
                OnlyTraceOf = ia.OnlyTraceOf,
            }).Distinct().ToListAsync(ctoken);

        return allergens;
    }

    public async Task UpdateAsync(IngredientDTO.UpdateRequest req)
    {
        var ingredient = await _ingredients.FindAsync(req.Id)
            ?? throw new NotFoundExcept("Ingredient not found!");

        ingredient.Name = req.Name;
        ingredient.Price = req.Price;
        ingredient.Description = req.Description;
        ingredient.InStock = req.InStock;

        ingredient.UpdatedAt = DateTime.UtcNow;

        await SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _ingredients.AnyAsync(i => i.Id == id))
            throw new NotFoundExcept("Ingredient not found!");

        _ingredients.Remove(new Ingredient { Id = id });
        await SaveAsync();
    }

    public async Task AddRemoveAllergenAsync(int id, IngredientDTO.AddRemoveAllergenRequest req)
    {
        var ingredientId = id;
        var allergenId = req.AllergenId;

        var exists = await _ingredients
            .Where(i => i.Id == ingredientId)
            .Select(i => new
            {
                IngredientExists = true,
                AllergenExists = _context.Allergens.Any(i => i.Id == allergenId),
                IngredientAllergenExists = _context.IngredientAllergens
                    .Any(ia => ia.IngredientId == ingredientId && ia.AllergenId == allergenId)
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundExcept("Ingredient not found!");

        if (!exists.AllergenExists) throw new NotFoundExcept("Allergen not found!");

        var ia = new IngredientAllergen
        {
            IngredientId = ingredientId,
            AllergenId = allergenId,
            OnlyTraceOf = req.OnlyTraceOf,
        };

        if (req.Add)
        {
            if (exists.IngredientAllergenExists) throw new BadRequestExcept("Allergen already added!");
            _context.IngredientAllergens.Add(ia);
        }
        else
        {
            if (!exists.IngredientAllergenExists) throw new BadRequestExcept("Allergen not added!");
            _context.IngredientAllergens.Remove(ia);
        }

        await SaveAsync();
    }

    public async Task ChangeStockAsync(int id, bool inStock)
    {
        if (!await _ingredients.AnyAsync(i => i.Id == id))
            throw new NotFoundExcept("Ingredient not found!");

        var currentDateTime = DateTime.UtcNow;

        await _ingredients
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(i => i.InStock, inStock)
                .SetProperty(i => i.UpdatedAt, currentDateTime)
            );
    }

    public async Task<List<IngredientDTO.Response>> GetAllAsync(CancellationToken ctoken)
    {
        var l = await _ingredients.Select(i => new IngredientDTO.Response
        {
            Id = i.Id,
            Price = i.Price,
            Name = i.Name,
            Description = i.Description,
            InStock = i.InStock,
        }).ToListAsync(ctoken);

        return l;
    }
}
