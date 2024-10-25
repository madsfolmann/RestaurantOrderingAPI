using RestaurantOrderingAPI.Application.Configurations;
using Microsoft.AspNetCore.Identity;
using RestaurantOrderingAPI.Application.Data;
using RestaurantOrderingAPI.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddAuthenticationServices();
builder.AddAuthorizationServices();
builder.AddSwaggerServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await AppDbContextDataSeed.SeedTestUsersAsync(userManager, roleManager);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();