using System;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Features.Allergens;

public class Allergen
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
