# Restaurant Ordering API
An ASP.NET Core WebAPI (.NET 8) for digitalising a restaurant: Includes user management, product catalog, ingredients and allergens, basket and order processing, JWT authentication, and a comprehensive set of endpoints.

# Features

- User Management: Includes customers, chefs, staff, admin, and superuser roles with pre-seeded test users.
- Products and Categories: Create new products and organize into categories.
- Ingredients and Allergens: Manage ingredients and their associated allergens.
- Basket and Ordering: Uses baskets, basket items, and a full ordering workflow.
- Authentication: Secured using JWT, with short-lived access tokens combined with long-lived refresh tokens.
- Authorization: Uses a combination of policy- and role-based authorization for endpoint access control.
- Feature-Based Architecture: Organized by features (e.g., Users, Orders, Baskets), each with dedicated service, model, DTO, and controller files.

# Quick Start

1. Clone the Repository and navigate to the project directory.
2. Configure appsettings.json: Add your JWT signing key and update the SQL Server connection string.
3. Run the Application: Use dotnet run to start in development mode, with initial seeding of roles and test users.
