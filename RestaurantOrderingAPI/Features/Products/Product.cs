using RestaurantOrderingAPI.Features.Categories;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Features.Products;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
}
