using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Application.Filters;
using RestaurantOrderingAPI.Features.Allergens;
using RestaurantOrderingAPI.Features.Auth;
using RestaurantOrderingAPI.Features.BasketItems;
using RestaurantOrderingAPI.Features.Baskets;
using RestaurantOrderingAPI.Features.Categories;
using RestaurantOrderingAPI.Features.Ingredients;
using RestaurantOrderingAPI.Features.Orders;
using RestaurantOrderingAPI.Features.Products;
using RestaurantOrderingAPI.Features.Tokens;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Application.Configurations;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        //Scopes
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAllergenService, AllergenService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBasketItemService, BasketItemService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAuthService, AuthService>();
        //Scopes

        services.AddDbContext<AppDbContext>(x =>
        {
            x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

        services.AddControllers(x =>
        {
            x.Filters.Add<ControllerExceptionFilter>();
        });

        return services;
    }
}
