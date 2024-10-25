namespace RestaurantOrderingAPI.Features.Allergens;

public class AllergenDTO
{

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class CreateRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}