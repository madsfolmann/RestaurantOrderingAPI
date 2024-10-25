using System;
using RestaurantOrderingAPI.Features.Ingredients;
using RestaurantOrderingAPI.Features.Products;

namespace RestaurantOrderingAPI.Application.JoiningTables;

public class ProductIngredient
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
}
