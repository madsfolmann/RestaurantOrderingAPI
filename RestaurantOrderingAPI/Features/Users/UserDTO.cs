using System.ComponentModel.DataAnnotations;
using RestaurantOrderingAPI.Application.Common.Models;

namespace RestaurantOrderingAPI.Features.Users;

public class UserDTO
{
    public class Response
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class UpdateRequest
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
    }
    public class CreateRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passwords must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
        ErrorMessage = "Passwords must have at least one lowercase ('a'-'z'), one uppercase ('A'-'Z'), one digit ('0'-'9'), and one non-alphanumeric character.")]
        public string Password { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
    }

    public class Query : BaseQuery
    {
        public Query() : base(50) { }
        public bool? IsActive { get; set; }

    }
}
