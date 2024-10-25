using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;

namespace RestaurantOrderingAPI.Features.Allergens;

public class AllergenService(AppDbContext context) : IAllergenService
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<Allergen> _allergens = context.Set<Allergen>();
    private async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<int> CreateAsync(AllergenDTO.CreateRequest req)
    {
        var a = new Allergen
        {
            Name = req.Name,
            Description = req.Description
        };

        _allergens.Add(a);
        await SaveAsync();

        return a.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var count = await _allergens.Where(a => a.Id == id).ExecuteDeleteAsync();
        if (count == 0)
            throw new NotFoundExcept("Allergen not found!");
    }

    public async Task<List<AllergenDTO.Response>> GetAllAsync(CancellationToken ctoken)
    {
        var l = await _allergens.Select(a => new AllergenDTO.Response
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
        }).ToListAsync(ctoken);

        return l;
    }

    public async Task UpdateAsync(AllergenDTO.UpdateRequest req)
    {
        var a = await _allergens.FindAsync(req.Id)
            ?? throw new NotFoundExcept("Allergen not found!");

        a.Name = req.Name;
        a.Description = req.Description;

        await SaveAsync();
    }

}