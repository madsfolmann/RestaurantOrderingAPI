using RestaurantOrderingAPI.Features.BasketItems;

namespace RestaurantOrderingAPI.Features.Baskets;

public class BasketDTO
{
    public class Response
    {
        public int Id { get; set; }
        public List<Item> Items { get; set; } = [];
        public decimal TotalPrice { get; set; }
        public class Item
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public List<string> ExtraIngredients { get; set; } = [];
            public decimal TotalPrice { get; set; }
        }
    }
}
