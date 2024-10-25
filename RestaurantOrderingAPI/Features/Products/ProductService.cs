using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Extensions;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Application.JoiningTables;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Features.Products;

public class ProductService(
    AppDbContext context,
    ILogger<ProductService> logger) : IProductService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProductService> _logger = logger;
    private readonly DbSet<Product> _products = context.Set<Product>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<int> CreateAsync(ProductDTO.CreateRequest req)
    {
        var product = new Product
        {
            Name = req.Name,
            Price = req.Price,
            Description = req.Description,
            CategoryId = req.CategoryId,
            IsActive = false,
        };

        _products.Add(product);
        await SaveAsync();

        return product.Id;
    }

    public async Task AddRemoveIndgredientAsync(int id, ProductDTO.AddRemoveIngredientRequest req)
    {
        var productId = id;
        var ingredientId = req.IngredientId;

        var exists = await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => new
            {
                IngredientExists = _context.Ingredients.Any(i => i.Id == ingredientId),
                ProductIngredientExists = _context.ProductIngredients.Any(pi => pi.ProductId == productId && pi.IngredientId == ingredientId)
            })
            .FirstOrDefaultAsync()
            ?? throw new NotFoundExcept("Product not found!");

        if (!exists.IngredientExists) throw new NotFoundExcept("Ingredient not found!");

        var pi = new ProductIngredient
        {
            ProductId = productId,
            IngredientId = req.IngredientId
        };

        if (req.Add)
        {
            if (exists.ProductIngredientExists) throw new BadRequestExcept("Ingredient already added!");
            _context.ProductIngredients.Add(pi);
        }
        else
        {
            if (!exists.ProductIngredientExists) throw new BadRequestExcept("Ingredient not added!");
            _context.ProductIngredients.Remove(pi);
        }

        await SaveAsync();
    }

    public async Task ChangeActiveStatusAsync(int id, bool active)
    {
        if (!await _products.AnyAsync(i => i.Id == id))
            throw new NotFoundExcept("Product not found!");

        var currentDateTime = DateTime.UtcNow;

        await _products
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(i => i.IsActive, active)
                .SetProperty(i => i.UpdatedAt, currentDateTime)
            );
    }

    public async Task<ProductDTO.Response> GetByIdAsync(
        int id, bool requireIsActive, CancellationToken ctoken)
    {
        var product = await _products.AsNoTracking()
            .Where(p => p.Id == id
                && (!requireIsActive || p.IsActive))
            .Select(p => new ProductDTO.Response
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Ingredients = p.Ingredients.Select(i => new IngredientDTO.Response
                {
                    Id = i.Id,
                    Price = i.Price,
                    Name = i.Name,
                    Description = i.Description,
                    InStock = i.InStock,
                }).ToList(),
            }).FirstOrDefaultAsync(ctoken)
                ?? throw new NotFoundExcept("Product not found!");

        return product;
    }

    public async Task<List<ProductDTO.QueryResponse>> QueryAsync(
        ProductDTO.Query q, CancellationToken ctoken)
    {
        var l = _products.AsNoTracking().Select(p => new ProductDTO.QueryResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            IngredientsInStock = p.Ingredients.All(pi => pi.InStock == true),
            Ingredients = p.Ingredients.Select(pi => pi.Name).ToList(),
        }).AsQueryable();

        if (q.IsActive != null)
            l = l.Where(p => p.IsActive == q.IsActive);

        if (q.Search != null)
            l = l.Where(x => x.Name.Contains(q.Search));

        bool asc = q.Ascending;
        l = q.SortBy switch
        {
            nameof(Product.Name) => l.SortByDir(x => x.Name, asc),
            nameof(Product.Price) => l.SortByDir(x => x.Price, asc),
            _ => l.SortByDir(x => x.CreatedAt, asc)
        };

        l = l.Paginate(q.PageNumber, q.PageSize);

        return await l.ToListAsync(ctoken);
    }

    public async Task UpdateAsync(ProductDTO.UpdateRequest req)
    {
        var product = await _products.FindAsync(req.Id)
            ?? throw new NotFoundExcept("Product not found!");

        product.Name = req.Name;
        product.Price = req.Price;
        product.Description = req.Description;
        product.IsActive = req.IsActive;
        product.CategoryId = req.CategoryId;

        product.UpdatedAt = DateTime.UtcNow;

        await SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var count = await _products.Where(p => p.Id == id).ExecuteDeleteAsync();

        if (count == 0)
            throw new NotFoundExcept("Product not found!");

        _logger.LogInformation("Deleted product {ProductId}.", id);
    }
}
