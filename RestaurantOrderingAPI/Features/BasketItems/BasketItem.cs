using RestaurantOrderingAPI.Application.JoiningTables;
using RestaurantOrderingAPI.Features.Baskets;
using RestaurantOrderingAPI.Features.Ingredients;
using RestaurantOrderingAPI.Features.Products;

namespace RestaurantOrderingAPI.Features.BasketItems;

public class BasketItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int BasketId { get; set; }
    public Basket Basket { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public List<Ingredient> ExtraIngredients { get; set; } = [];
    public List<BasketItemIngredient> BasketItemIngredients { get; set; }
}
