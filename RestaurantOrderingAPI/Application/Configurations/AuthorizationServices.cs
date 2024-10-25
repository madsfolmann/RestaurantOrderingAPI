using RestaurantOrderingAPI.Application.Common.Constants;

namespace RestaurantOrderingAPI.Application.Configurations;

public static class AuthorizationServices
{
    public static IServiceCollection AddAuthorizationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CanManageFood, policy => policy.RequireRole(Role.Admin, Role.Staff));
        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CanManageOrders, policy => policy.RequireRole(Role.Admin, Role.Staff));
        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CanManageUsers, policy => policy.RequireRole(Role.Admin));
        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CanReadUsers, policy => policy.RequireRole(Role.Admin, Role.Staff));
        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CanManageKitchenOrders, policy => policy.RequireRole(Role.Admin, Role.Staff, Role.Chef));

        return services;
    }
}
