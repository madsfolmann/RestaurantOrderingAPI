using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;

namespace RestaurantOrderingAPI.Features.Categories;

public class CategoryService(AppDbContext context) : ICategoryService
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<Category> _categories = context.Set<Category>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<int> CreateAsync(CategoryDTO.CreateRequest req)
    {
        var a = new Category
        {
            Name = req.Name,
        };

        _categories.Add(a);
        await SaveAsync();

        return a.Id;
    }

    public async Task DeleteAsync(int id)
    {
        if (await _context.Products.AllAsync(p => p.CategoryId == id))
            throw new BadRequestExcept("Category with products cannot be deleted!");

        var count = await _categories.Where(c => c.Id == id).ExecuteDeleteAsync();
        if (count == 0)
            throw new NotFoundExcept("Category not found!");
    }

    public async Task<List<CategoryDTO.Response>> GetAllAsync(CancellationToken ctoken)
    {
        var l = await _categories.Select(a => new CategoryDTO.Response
        {
            Id = a.Id,
            Name = a.Name,
            AttachedProducts = a.Products.Select(p => new CategoryDTO.Response.Product
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList(),
        }).ToListAsync(ctoken);

        return l;
    }

    public async Task UpdateAsync(CategoryDTO.UpdateRequest req)
    {
        var a = await _categories.FindAsync(req.Id)
            ?? throw new NotFoundExcept("Category not found!");

        a.Name = req.Name;

        await SaveAsync();
    }
}