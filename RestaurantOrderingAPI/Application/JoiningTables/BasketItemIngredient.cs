using System;
using RestaurantOrderingAPI.Features.BasketItems;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Application.JoiningTables;

public class BasketItemIngredient
{
    public int BasketItemId { get; set; }
    public BasketItem BasketItem { get; set; }
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
    public int Quantity { get; set; }
}
