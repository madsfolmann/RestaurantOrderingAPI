using RestaurantOrderingAPI.Features.Products;

namespace RestaurantOrderingAPI.Features.Categories;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
}
