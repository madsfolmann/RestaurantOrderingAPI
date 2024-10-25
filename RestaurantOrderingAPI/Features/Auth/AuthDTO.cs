using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingAPI.Features.Auth;

public class AuthDTO
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
