using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingAPI.Features.BasketItems;

public class BasketItemDTO
{
    public class CreateRequest
    {
        public int ProductId { get; set; }
        public int BasketId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateIngredientQuantityRequest
    {
        public int IngredientId { get; set; }

        [Range(0, 3)]
        public int Quantity { get; set; }
    }
}