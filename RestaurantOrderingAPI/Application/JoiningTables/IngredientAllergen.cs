using RestaurantOrderingAPI.Features.Allergens;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Application.JoiningTables;

public class IngredientAllergen
{
    public int AllergenId { get; set; }
    public Allergen Allergen { get; set; }
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
    public bool OnlyTraceOf { get; set; }
}
