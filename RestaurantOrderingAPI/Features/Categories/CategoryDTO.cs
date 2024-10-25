namespace RestaurantOrderingAPI.Features.Categories;

public class CategoryDTO
{
    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> AttachedProducts { get; set; }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
    public class UpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CreateRequest
    {
        public string Name { get; set; }
    }
}
