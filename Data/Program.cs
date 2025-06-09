using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Cafeteria Management System...");

        using (var context = new MyContext())
        {
            try
            {
                // Check if the database is empty and needs seeding
                bool needsSeeding = await context.Database.EnsureCreatedAsync();

                if (needsSeeding || !await context.Categories.AnyAsync())
                {
                    Console.WriteLine("Seeding database with initial data...");
                    await DatabaseSeeder.SeedDataAsync(context);
                    Console.WriteLine("Database seeded successfully!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }

                // Create and run the menu service
                var menuService = new MenuService(context);
                await menuService.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}