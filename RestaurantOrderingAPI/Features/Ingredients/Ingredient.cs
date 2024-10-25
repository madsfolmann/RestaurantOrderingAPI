using RestaurantOrderingAPI.Application.JoiningTables;
using RestaurantOrderingAPI.Features.Allergens;
using RestaurantOrderingAPI.Features.Products;

namespace RestaurantOrderingAPI.Features.Ingredients;

public class Ingredient
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool InStock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<Allergen> Allergens { get; set; } = [];
    public List<Product> Products { get; set; } = [];
    public List<IngredientAllergen> IngredientAllergens { get; set; } = [];
}
