using RestaurantOrderingAPI.Features.Allergens;

namespace RestaurantOrderingAPI.Features.Ingredients;

public class IngredientDTO
{

    public class Response
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool InStock { get; set; }
    }

    public class CreateRequest
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateRequest
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool InStock { get; set; }
    }

    public class AddRemoveAllergenRequest
    {
        public int AllergenId { get; set; }
        public bool Add { get; set; }
        public bool OnlyTraceOf { get; set; }
    }


    public class AllergenResponse
    {
        public AllergenDTO.Response Allergen { get; set; }
        public bool OnlyTraceOf { get; set; }
    }
}
