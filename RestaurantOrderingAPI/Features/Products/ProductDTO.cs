using RestaurantOrderingAPI.Application.Common.Models;
using RestaurantOrderingAPI.Features.Ingredients;

namespace RestaurantOrderingAPI.Features.Products;
public class ProductDTO
{
    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<IngredientDTO.Response> Ingredients { get; set; } = [];
    }

    public class UpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
    }

    public class QueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IngredientsInStock { get; set; }
        public List<string> Ingredients { get; set; }
    }

    public class Query : BaseQuery
    {
        public Query() : base(30) { }
        public bool? IsActive { get; set; }
    }

    public class CreateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class AddRemoveIngredientRequest
    {
        public int IngredientId { get; set; }
        public bool Add { get; set; }
    }
}