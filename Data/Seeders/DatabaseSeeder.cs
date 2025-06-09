using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(MyContext context)
    {
        // Create categories
        await SeedCategoriesAsync(context);

        // Create tables
        await SeedTablesAsync(context);

        // Create workers
        await SeedWorkersAsync(context);

        // Create items
        await SeedItemsAsync(context);
    }

    private static async Task SeedCategoriesAsync(MyContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new Category { Name = "Hot Drinks", Description = "Coffee, tea, and other hot beverages" },
            new Category { Name = "Cold Drinks", Description = "Refreshing cold beverages and sodas" },
            new Category { Name = "Sandwiches", Description = "Fresh sandwiches and wraps" },
            new Category { Name = "Desserts", Description = "Sweet treats and desserts" },
            new Category { Name = "Salads", Description = "Healthy salads and bowls" },
            new Category { Name = "Breakfast", Description = "Morning breakfast items" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTablesAsync(MyContext context)
    {
        if (await context.Tables.AnyAsync())
            return;

        var tables = new List<Table>
        {
            new Table { TableNumber = "A1", Capacity = 2, IsOccupied = false },
            new Table { TableNumber = "A2", Capacity = 2, IsOccupied = false },
            new Table { TableNumber = "B1", Capacity = 4, IsOccupied = false },
            new Table { TableNumber = "B2", Capacity = 4, IsOccupied = false },
            new Table { TableNumber = "C1", Capacity = 6, IsOccupied = false },
            new Table { TableNumber = "D1", Capacity = 8, IsOccupied = false }
        };

        await context.Tables.AddRangeAsync(tables);
        await context.SaveChangesAsync();
    }

    private static async Task SeedWorkersAsync(MyContext context)
    {
        if (await context.Workers.AnyAsync())
            return;

        var workers = new List<Worker>
        {
            new Worker
            {
                Name = "Admin User",
                Username = "admin",
                Password = "admin123", // In production, use hashed passwords
                Role = WorkerRole.Admin,
                IsActive = true
            },
            new Worker
            {
                Name = "John Manager",
                Username = "john",
                Password = "john123",
                Role = WorkerRole.Manager,
                IsActive = true
            },
            new Worker
            {
                Name = "Sarah Cashier",
                Username = "sarah",
                Password = "sarah123",
                Role = WorkerRole.Cashier,
                IsActive = true
            },
            new Worker
            {
                Name = "Mike Chef",
                Username = "mike",
                Password = "mike123",
                Role = WorkerRole.Chef,
                IsActive = true
            }
        };

        await context.Workers.AddRangeAsync(workers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedItemsAsync(MyContext context)
    {
        if (await context.Items.AnyAsync())
            return;

        // Get category IDs
        var hotDrinks = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Hot Drinks");
        var coldDrinks = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Cold Drinks");
        var sandwiches = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Sandwiches");
        var desserts = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Desserts");
        var salads = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Salads");
        var breakfast = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Breakfast");

        if (hotDrinks == null || coldDrinks == null || sandwiches == null || 
            desserts == null || salads == null || breakfast == null)
        {
            throw new Exception("Categories not found. Please seed categories first.");
        }

        var items = new List<Item>
        {
            // Hot Drinks
            new Item
            {
                Name = "Espresso",
                Description = "Strong coffee brewed by forcing steam through ground coffee beans",
                Price = 2.50m,
                CategoryId = hotDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 50
            },
            new Item
            {
                Name = "Cappuccino",
                Description = "Espresso with steamed milk foam",
                Price = 3.50m,
                CategoryId = hotDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 40
            },
            new Item
            {
                Name = "Latte",
                Description = "Espresso with steamed milk",
                Price = 3.75m,
                CategoryId = hotDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 45
            },
            new Item
            {
                Name = "Green Tea",
                Description = "Traditional green tea",
                Price = 2.25m,
                CategoryId = hotDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 60
            },

            // Cold Drinks
            new Item
            {
                Name = "Iced Coffee",
                Description = "Chilled coffee served with ice",
                Price = 3.25m,
                CategoryId = coldDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 35
            },
            new Item
            {
                Name = "Fresh Orange Juice",
                Description = "Freshly squeezed orange juice",
                Price = 3.95m,
                CategoryId = coldDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 25
            },
            new Item
            {
                Name = "Strawberry Smoothie",
                Description = "Blended strawberries with yogurt and honey",
                Price = 4.50m,
                CategoryId = coldDrinks.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 20
            },

            // Sandwiches
            new Item
            {
                Name = "Chicken Sandwich",
                Description = "Grilled chicken with lettuce and mayo",
                Price = 6.95m,
                CategoryId = sandwiches.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 15
            },
            new Item
            {
                Name = "Veggie Wrap",
                Description = "Mixed vegetables with hummus in a wrap",
                Price = 5.95m,
                CategoryId = sandwiches.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 12
            },
            new Item
            {
                Name = "Tuna Melt",
                Description = "Tuna with melted cheese on toasted bread",
                Price = 7.25m,
                CategoryId = sandwiches.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 10
            },

            // Desserts
            new Item
            {
                Name = "Chocolate Cake",
                Description = "Rich chocolate layer cake",
                Price = 4.95m,
                CategoryId = desserts.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 8
            },
            new Item
            {
                Name = "Cheesecake",
                Description = "Classic New York style cheesecake",
                Price = 5.25m,
                CategoryId = desserts.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 6
            },
            new Item
            {
                Name = "Fruit Tart",
                Description = "Pastry shell with custard and fresh fruits",
                Price = 4.75m,
                CategoryId = desserts.CategoryId,
                Status = ItemStatus.Seasonal,
                StockQuantity = 5
            },

            // Salads
            new Item
            {
                Name = "Caesar Salad",
                Description = "Romaine lettuce with Caesar dressing and croutons",
                Price = 8.50m,
                CategoryId = salads.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 10
            },
            new Item
            {
                Name = "Greek Salad",
                Description = "Mixed greens with feta, olives, and Greek dressing",
                Price = 8.95m,
                CategoryId = salads.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 8
            },

            // Breakfast
            new Item
            {
                Name = "Eggs Benedict",
                Description = "Poached eggs with hollandaise sauce on English muffin",
                Price = 9.95m,
                CategoryId = breakfast.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 15
            },
            new Item
            {
                Name = "Avocado Toast",
                Description = "Smashed avocado on toast with poached egg",
                Price = 8.50m,
                CategoryId = breakfast.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 12
            },
            new Item
            {
                Name = "Pancake Stack",
                Description = "Stack of fluffy pancakes with maple syrup",
                Price = 7.95m,
                CategoryId = breakfast.CategoryId,
                Status = ItemStatus.Available,
                StockQuantity = 18
            }
        };

        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
}
