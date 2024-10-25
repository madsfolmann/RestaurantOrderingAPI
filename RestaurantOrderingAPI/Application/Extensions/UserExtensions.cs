using System.Security.Claims;
using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Application.Extensions
{
    public static class UserExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimType.UserId);
        }
    }
}