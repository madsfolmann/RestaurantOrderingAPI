using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Application.JoiningTables;
using RestaurantOrderingAPI.Features.Allergens;
using RestaurantOrderingAPI.Features.BasketItems;
using RestaurantOrderingAPI.Features.Baskets;
using RestaurantOrderingAPI.Features.Categories;
using RestaurantOrderingAPI.Features.Ingredients;
using RestaurantOrderingAPI.Features.Orders;
using RestaurantOrderingAPI.Features.Products;
using RestaurantOrderingAPI.Features.Tokens;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Application.Data;

public class AppDbContext(DbContextOptions dbContextOptions) : IdentityDbContext<User>(dbContextOptions)
{
    //DbSets
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Allergen> Allergens { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<IngredientAllergen> IngredientAllergens { get; set; }
    public DbSet<ProductIngredient> ProductIngredients { get; set; }
    public DbSet<BasketItemIngredient> BasketItemIngredients { get; set; }
    //DbSets

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Basket>()
        .HasOne(x => x.User)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Ingredient>()
        .HasMany(x => x.Allergens)
        .WithMany()
        .UsingEntity<IngredientAllergen>();

        builder.Entity<Product>()
        .HasMany(x => x.Ingredients)
        .WithMany(x => x.Products)
        .UsingEntity<ProductIngredient>();

        builder.Entity<BasketItem>()
        .HasMany(x => x.ExtraIngredients)
        .WithMany()
        .UsingEntity<BasketItemIngredient>();

        builder.Entity<IdentityRole>().HasData(roles);
    }

    private readonly List<IdentityRole> roles = [
        new IdentityRole
        {
            Id = "34985620-22ac-45e4-9f77-02b9d3fa3cb1",
            Name = Role.Customer,
            NormalizedName = Role.Customer.ToUpper(),
        },
        new IdentityRole
        {
            Id = "d27b616f-a95b-4874-a258-0f3f2f1a8977",
            Name = Role.Chef,
            NormalizedName = Role.Chef.ToUpper(),
        },
        new IdentityRole
        {
            Id = "d3011823-1fde-4007-ba8e-701ed5844fda",
            Name = Role.Staff,
            NormalizedName = Role.Staff.ToUpper(),
        },
        new IdentityRole
        {
            Id = "fd31c057-53e5-42af-9b91-464800e8d4c5",
            Name = Role.Admin,
            NormalizedName = Role.Admin.ToUpper(),
        },
        new IdentityRole
        {
            Id = "gd31c057-53e5-42af-9b91-464800e8d4c5",
            Name = Role.SuperUser,
            NormalizedName = Role.SuperUser.ToUpper(),
        },
    ];
}