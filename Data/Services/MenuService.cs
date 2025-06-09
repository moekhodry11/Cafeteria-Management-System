using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;

namespace Data;

public class MenuService
{
    private readonly MyContext _context;

    public MenuService(MyContext context)
    {
        _context = context;
    }

    public async Task RunAsync()
    {
        bool exit = false;

        while (!exit)
        {
            ShowMainMenu();
            Console.Write("\nSelect an option (1-5): ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ManageItemsMenuAsync();
                    break;
                case "2":
                    await ManageOrdersMenuAsync();
                    break;
                case "3":
                    await ManageWorkersMenuAsync();
                    break;
                case "4":
                    await ViewReportsMenuAsync();
                    break;
                case "5":
                    exit = true;
                    Console.WriteLine("Thank you for using Cafeteria Management System!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    public void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Cafeteria Management System ===");
        Console.WriteLine("1. Manage Items");
        Console.WriteLine("2. Manage Orders");
        Console.WriteLine("3. Manage Workers");
        Console.WriteLine("4. View Reports");
        Console.WriteLine("5. Exit");
    }

    #region Item Management

    private async Task ManageItemsMenuAsync()
    {
        bool back = false;

        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== Manage Items ===");
            Console.WriteLine("1. View All Items");
            Console.WriteLine("2. Add New Item");
            Console.WriteLine("3. Update Item");
            Console.WriteLine("4. Delete Item");
            Console.WriteLine("5. View Items by Category");
            Console.WriteLine("6. Change Item Status");
            Console.WriteLine("7. Back to Main Menu");

            Console.Write("\nSelect an option (1-7): ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ViewAllItemsAsync();
                    break;
                case "2":
                    await AddNewItemAsync();
                    break;
                case "3":
                    await UpdateItemAsync();
                    break;
                case "4":
                    await DeleteItemAsync();
                    break;
                case "5":
                    await ViewItemsByCategoryAsync();
                    break;
                case "6":
                    await ChangeItemStatusAsync();
                    break;
                case "7":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!back)
            {
                Console.WriteLine("\nPress any key to return to the Items menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task ViewAllItemsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Items ===");

        var items = await _context.Items.Include(i => i.Category).ToListAsync();

        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }

        Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10} {4,-15} {5,-15}", "ID", "Name", "Price", "Stock", "Status", "Category");
        Console.WriteLine(new string('-', 80));

        foreach (var item in items)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-10} {4,-15} {5,-15}", 
                item.ItemId, 
                item.Name, 
                item.Price, 
                item.StockQuantity,
                item.Status, 
                item.Category?.Name ?? "No Category");
        }
    }

    private async Task AddNewItemAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Item ===");

        try
        {
            // Display available categories
            var categories = await _context.Categories.ToListAsync();

            if (categories.Count == 0)
            {
                Console.WriteLine("No categories found. Please add a category first.");
                await AddNewCategoryAsync();
                categories = await _context.Categories.ToListAsync();
            }

            Console.WriteLine("\nAvailable Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId}. {category.Name}");
            }

            Console.Write("\nSelect Category (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId) ||
                !categories.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine("Invalid category selection.");
                return;
            }

            // Get item details
            Console.Write("Item Name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Item name cannot be empty.");
                return;
            }

            Console.Write("Item Description: ");
            string? description = Console.ReadLine();

            decimal price = 0;
            bool validPrice = false;
            while (!validPrice)
            {
                Console.Write("Price: ");
                validPrice = decimal.TryParse(Console.ReadLine(), out price);

                if (!validPrice || price < 0)
                {
                    Console.WriteLine("Please enter a valid price.");
                    validPrice = false;
                }
            }

            // Display available statuses
            Console.WriteLine("\nAvailable Item Statuses:");
            foreach (ItemStatus status in Enum.GetValues(typeof(ItemStatus)))
            {
                Console.WriteLine($"{(int)status}. {status}");
            }

            Console.Write("Select Item Status (number): ");
            if (!int.TryParse(Console.ReadLine(), out int statusValue) ||
                !Enum.IsDefined(typeof(ItemStatus), statusValue))
            {
                Console.WriteLine("Invalid status. Setting to Available by default.");
                statusValue = (int)ItemStatus.Available;
            }

            // Get stock quantity
            int stockQuantity = 0;
            bool validStock = false;
            while (!validStock)
            {
                Console.Write("Stock Quantity: ");
                validStock = int.TryParse(Console.ReadLine(), out stockQuantity);

                if (!validStock || stockQuantity < 0)
                {
                    Console.WriteLine("Please enter a valid stock quantity.");
                    validStock = false;
                }
            }

            // Create new item
            var newItem = new Item
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                StockQuantity = stockQuantity,
                Status = (ItemStatus)statusValue
            };

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nItem '{name}' added successfully with ID: {newItem.ItemId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding item: {ex.Message}");
        }
    }

    private async Task UpdateItemAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Item ===");

        try
        {
            await ViewAllItemsAsync();

            Console.Write("\nEnter Item ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var item = await _context.Items.FindAsync(itemId);

            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.WriteLine($"\nUpdating Item: {item.Name}");

            Console.Write($"New Name (current: {item.Name}): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                item.Name = name;
            }

            Console.Write($"New Description (current: {item.Description}): ");
            string? description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description))
            {
                item.Description = description;
            }

            Console.Write($"New Price (current: {item.Price:C}): ");
            string? priceInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
            {
                item.Price = price;
            }

            Console.Write($"New Stock Quantity (current: {item.StockQuantity}): ");
            string? stockInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(stockInput) && int.TryParse(stockInput, out int stockQuantity) && stockQuantity >= 0)
            {
                item.StockQuantity = stockQuantity;

                // Update item status based on stock quantity
                if (stockQuantity == 0 && item.Status == ItemStatus.Available)
                {
                    item.Status = ItemStatus.OutOfStock;
                }
                else if (stockQuantity > 0 && item.Status == ItemStatus.OutOfStock)
                {
                    item.Status = ItemStatus.Available;
                }
            }

            // Display available categories
            var categories = await _context.Categories.ToListAsync();
            Console.WriteLine("\nAvailable Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId}. {category.Name}");
            }

            Console.Write($"New Category ID (current: {item.CategoryId}): ");
            string? categoryInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(categoryInput) && int.TryParse(categoryInput, out int categoryId))
            {
                if (categories.Any(c => c.CategoryId == categoryId))
                {
                    item.CategoryId = categoryId;
                }
                else
                {
                    Console.WriteLine("Invalid category ID. Category not updated.");
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("\nItem updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating item: {ex.Message}");
        }
    }

    private async Task DeleteItemAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Item ===");

        try
        {
            await ViewAllItemsAsync();

            Console.Write("\nEnter Item ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var item = await _context.Items.FindAsync(itemId);

            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            // Check if the item is used in any orders
            bool isUsedInOrders = await _context.OrderItems.AnyAsync(oi => oi.ItemId == itemId);

            if (isUsedInOrders)
            {
                Console.WriteLine("Cannot delete this item as it is used in existing orders.");
                Console.WriteLine("Would you like to mark it as Discontinued instead? (y/n)");

                if (Console.ReadLine()?.ToLower() == "y")
                {
                    item.Status = ItemStatus.Discontinued;
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Item '{item.Name}' marked as Discontinued.");
                }

                return;
            }

            Console.Write($"Are you sure you want to delete '{item.Name}'? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                Console.WriteLine("\nItem deleted successfully!");
            }
            else
            {
                Console.WriteLine("\nDeletion cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting item: {ex.Message}");
        }
    }

    private async Task ViewItemsByCategoryAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View Items by Category ===");

        try
        {
            var categories = await _context.Categories.ToListAsync();

            if (categories.Count == 0)
            {
                Console.WriteLine("No categories found.");
                return;
            }

            Console.WriteLine("\nAvailable Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId}. {category.Name}");
            }

            Console.Write("\nSelect Category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var selectedCategory = await _context.Categories.FindAsync(categoryId);

            if (selectedCategory == null)
            {
                Console.WriteLine("Category not found.");
                return;
            }

            var items = await _context.Items
                .Where(i => i.CategoryId == categoryId)
                .ToListAsync();

            Console.Clear();
            Console.WriteLine($"=== Items in Category: {selectedCategory.Name} ===");

            if (items.Count == 0)
            {
                Console.WriteLine("No items found in this category.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10} {4,-15}", "ID", "Name", "Price", "Stock", "Status");
            Console.WriteLine(new string('-', 65));

            foreach (var item in items)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-10} {4,-15}", 
                    item.ItemId, 
                    item.Name, 
                    item.Price, 
                    item.StockQuantity,
                    item.Status);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error viewing items by category: {ex.Message}");
        }
    }

    private async Task ChangeItemStatusAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Change Item Status ===");

        try
        {
            await ViewAllItemsAsync();

            Console.Write("\nEnter Item ID to change status: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var item = await _context.Items.FindAsync(itemId);

            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.WriteLine($"\nCurrent Status of '{item.Name}': {item.Status}");

            Console.WriteLine("\nAvailable Item Statuses:");
            foreach (ItemStatus status in Enum.GetValues(typeof(ItemStatus)))
            {
                Console.WriteLine($"{(int)status}. {status}");
            }

            Console.Write("\nSelect new status (number): ");
            if (!int.TryParse(Console.ReadLine(), out int statusValue) ||
                !Enum.IsDefined(typeof(ItemStatus), statusValue))
            {
                Console.WriteLine("Invalid status selected.");
                return;
            }

            item.Status = (ItemStatus)statusValue;

            // Adjust stock quantity based on status change
            if (item.Status == ItemStatus.OutOfStock && item.StockQuantity > 0)
            {
                Console.WriteLine($"Warning: Setting item to Out of Stock will set stock quantity to 0.");
                Console.Write("Continue? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    item.StockQuantity = 0;
                }
                else
                {
                    Console.WriteLine("Status change cancelled.");
                    return;
                }
            }
            else if (item.Status == ItemStatus.Available && item.StockQuantity == 0)
            {
                Console.Write("Enter new stock quantity: ");
                if (int.TryParse(Console.ReadLine(), out int newStock) && newStock > 0)
                {
                    item.StockQuantity = newStock;
                }
                else
                {
                    Console.WriteLine("Invalid stock quantity. Item cannot be Available with 0 stock.");
                    return;
                }
            }

            // Update modified date
            item.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            Console.WriteLine($"\nStatus of '{item.Name}' updated to {item.Status} successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing item status: {ex.Message}");
        }
    }

    private async Task AddNewCategoryAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Category ===");

        try
        {
            Console.Write("Category Name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                return;
            }

            Console.Write("Category Description: ");
            string? description = Console.ReadLine();

            var newCategory = new Category
            {
                Name = name,
                Description = description ?? string.Empty
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nCategory '{name}' added successfully with ID: {newCategory.CategoryId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding category: {ex.Message}");
        }
    }

    #endregion

    #region Order Management

    private async Task ManageOrdersMenuAsync()
    {
        bool back = false;

        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== Manage Orders ===");
            Console.WriteLine("1. View All Orders");
            Console.WriteLine("2. Create New Order");
            Console.WriteLine("3. Add Items to Order");
            Console.WriteLine("4. Update Order Status");
            Console.WriteLine("5. Process Payment");
            Console.WriteLine("6. View Order Details");
            Console.WriteLine("7. Cancel Order");
            Console.WriteLine("8. Back to Main Menu");

            Console.Write("\nSelect an option (1-8): ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ViewAllOrdersAsync();
                    break;
                case "2":
                    await CreateNewOrderAsync();
                    break;
                case "3":
                    await AddItemsToOrderAsync();
                    break;
                case "4":
                    await UpdateOrderStatusAsync();
                    break;
                case "5":
                    await ProcessPaymentAsync();
                    break;
                case "6":
                    await ViewOrderDetailsAsync();
                    break;
                case "7":
                    await CancelOrderAsync();
                    break;
                case "8":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!back)
            {
                Console.WriteLine("\nPress any key to return to the Orders menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task ViewAllOrdersAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Orders ===");

        var orders = await _context.Orders
            .Include(o => o.Worker)
            .Include(o => o.Table)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
            return;
        }

        Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15} {4,-10} {5,-15}", 
            "ID", "Date", "Amount", "Status", "Paid", "Worker");
        Console.WriteLine(new string('-', 80));

        foreach (var order in orders)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15} {4,-10} {5,-15}", 
                order.OrderId, 
                order.OrderDate.ToString("g"), 
                order.TotalAmount, 
                order.Status, 
                order.IsPaid ? "Yes" : "No", 
                order.Worker?.Name ?? "Unknown");
        }
    }

    private async Task CreateNewOrderAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create New Order ===");

        try
        {
            // Get workers
            var workers = await _context.Workers.Where(w => w.IsActive).ToListAsync();

            if (workers.Count == 0)
            {
                Console.WriteLine("No active workers found. Please add a worker first.");
                await AddNewWorkerAsync();
                workers = await _context.Workers.Where(w => w.IsActive).ToListAsync();
            }

            Console.WriteLine("\nAvailable Workers:");
            foreach (var worker in workers)
            {
                Console.WriteLine($"{worker.WorkerId}. {worker.Name} ({worker.Role})");
            }

            Console.Write("\nSelect Worker ID: ");
            if (!int.TryParse(Console.ReadLine(), out int workerId) ||
                !workers.Any(w => w.WorkerId == workerId))
            {
                Console.WriteLine("Invalid worker ID.");
                return;
            }

            // Get tables
            var tables = await _context.Tables.Where(t => !t.IsOccupied).ToListAsync();

            Console.WriteLine("\nAvailable Tables:");
            Console.WriteLine("0. No Table (Take-away)");

            foreach (var table in tables)
            {
                Console.WriteLine($"{table.TableId}. Table {table.TableNumber} (Capacity: {table.Capacity})");
            }

            Console.Write("\nSelect Table ID (0 for no table): ");
            int? tableId = null;
            string? tableInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(tableInput))
            {
                if (int.TryParse(tableInput, out int tId))
                {
                    if (tId != 0)
                    {
                        if (tables.Any(t => t.TableId == tId))
                        {
                            tableId = tId;

                            // Mark table as occupied
                            var selectedTable = await _context.Tables.FindAsync(tableId);
                            if (selectedTable != null)
                            {
                                selectedTable.IsOccupied = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid table ID. Creating order without a table.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Creating order without a table.");
                }
            }

            // Create the order
            var newOrder = new Order
            {
                WorkerId = workerId,
                TableId = tableId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nOrder created successfully with ID: {newOrder.OrderId}");
            Console.WriteLine("Would you like to add items to this order now? (y/n)");

            if (Console.ReadLine()?.ToLower() == "y")
            {
                await AddItemsToSpecificOrderAsync(newOrder.OrderId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating order: {ex.Message}");
        }
    }

    private async Task AddItemsToOrderAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Add Items to Order ===");

        try
        {
            // Get pending orders
            var pendingOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("No pending or in-progress orders found.");
                Console.WriteLine("Would you like to create a new order? (y/n)");

                if (Console.ReadLine()?.ToLower() == "y")
                {
                    await CreateNewOrderAsync();
                }

                return;
            }

            Console.WriteLine("\nPending Orders:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15}", "ID", "Date", "Amount", "Status");
            Console.WriteLine(new string('-', 55));

            foreach (var order in pendingOrders)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15}", 
                    order.OrderId, 
                    order.OrderDate.ToString("g"), 
                    order.TotalAmount, 
                    order.Status);
            }

            Console.Write("\nSelect Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId) ||
                !pendingOrders.Any(o => o.OrderId == orderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            await AddItemsToSpecificOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding items to order: {ex.Message}");
        }
    }

    private async Task AddItemsToSpecificOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        bool addMoreItems = true;

        while (addMoreItems)
        {
            Console.Clear();
            Console.WriteLine($"=== Adding Items to Order #{orderId} ===");

            // Show available items
            var availableItems = await _context.Items
                .Where(i => i.Status == ItemStatus.Available && i.StockQuantity > 0)
                .Include(i => i.Category)
                .ToListAsync();

            if (availableItems.Count == 0)
            {
                Console.WriteLine("No available items found.");
                return;
            }

            Console.WriteLine("\nAvailable Items:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15}", "ID", "Name", "Price", "Category");
            Console.WriteLine(new string('-', 55));

            foreach (var item in availableItems)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15}", 
                    item.ItemId, 
                    item.Name, 
                    item.Price, 
                    item.Category?.Name ?? "No Category");
            }

            // Select item
            Console.Write("\nSelect Item ID: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId) ||
                !availableItems.Any(i => i.ItemId == itemId))
            {
                Console.WriteLine("Invalid item ID.");
                Console.WriteLine("Try again? (y/n)");

                if (Console.ReadLine()?.ToLower() != "y")
                {
                    addMoreItems = false;
                }

                continue;
            }

            var selectedItem = availableItems.First(i => i.ItemId == itemId);

            // Get quantity
            Console.Write($"Quantity for {selectedItem.Name} (available: {selectedItem.StockQuantity}, default: 1): ");
            string? quantityInput = Console.ReadLine();
            int quantity = 1;

            if (!string.IsNullOrWhiteSpace(quantityInput) && int.TryParse(quantityInput, out int qty))
            {
                quantity = qty > 0 ? qty : 1;
            }

            // Verify stock availability
            if (quantity > selectedItem.StockQuantity)
            {
                Console.WriteLine($"Warning: Only {selectedItem.StockQuantity} items in stock.");
                Console.Write($"Order {selectedItem.StockQuantity} instead? (y/n): ");

                if (Console.ReadLine()?.ToLower() == "y")
                {
                    quantity = selectedItem.StockQuantity;
                }
                else
                {
                    Console.WriteLine("Item not added to order.");
                    Console.WriteLine("Try again? (y/n)");

                    if (Console.ReadLine()?.ToLower() != "y")
                    {
                        addMoreItems = false;
                    }

                    continue;
                }
            }

            // Check if item already exists in order
            var existingOrderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ItemId == itemId);

            if (existingOrderItem != null)
            {
                existingOrderItem.Quantity += quantity;
                existingOrderItem.TotalPrice = existingOrderItem.Quantity * existingOrderItem.UnitPrice;
            }
            else
            {
                // Add new order item
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ItemId = itemId,
                    Quantity = quantity,
                    UnitPrice = selectedItem.Price,
                    TotalPrice = quantity * selectedItem.Price
                };

                _context.OrderItems.Add(orderItem);
            }

            // Update order total
            // Update stock quantity
            selectedItem.StockQuantity -= quantity;
            if (selectedItem.StockQuantity == 0)
            {
                selectedItem.Status = ItemStatus.OutOfStock;
            }

            await _context.SaveChangesAsync();

            // Recalculate order total
            order.TotalAmount = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.TotalPrice);

            // Update order status to InProgress if it was Pending
            if (order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.InProgress;
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"\n{quantity} x {selectedItem.Name} added to the order.");
            Console.WriteLine($"Current order total: {order.TotalAmount:C}");

            // Add more items?
            Console.WriteLine("\nAdd more items? (y/n)");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                addMoreItems = false;
            }
        }

        Console.WriteLine("\nOrder updated successfully!");
    }

    private async Task UpdateOrderStatusAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Order Status ===");

        try
        {
            var activeOrders = await _context.Orders
                .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (activeOrders.Count == 0)
            {
                Console.WriteLine("No active orders found.");
                return;
            }

            Console.WriteLine("\nActive Orders:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15}", "ID", "Date", "Amount", "Status");
            Console.WriteLine(new string('-', 55));

            foreach (var order in activeOrders)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15}", 
                    order.OrderId, 
                    order.OrderDate.ToString("g"), 
                    order.TotalAmount, 
                    order.Status);
            }

            Console.Write("\nSelect Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedOrderId) ||
                !activeOrders.Any(o => o.OrderId == selectedOrderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            var selectedOrder = activeOrders.First(o => o.OrderId == selectedOrderId);

            Console.WriteLine($"\nCurrent Status of Order #{selectedOrder.OrderId}: {selectedOrder.Status}");

            Console.WriteLine("\nAvailable Order Statuses:");
            foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
            {
                Console.WriteLine($"{(int)status}. {status}");
            }

            Console.Write("\nSelect new status (number): ");
            if (!int.TryParse(Console.ReadLine(), out int statusValue) ||
                !Enum.IsDefined(typeof(OrderStatus), statusValue))
            {
                Console.WriteLine("Invalid status selected.");
                return;
            }

            OrderStatus newStatus = (OrderStatus)statusValue;

            // Special handling for Completed status
            if (newStatus == OrderStatus.Completed)
            {
                if (!selectedOrder.IsPaid)
                {
                    Console.WriteLine("\nWarning: This order has not been paid yet.");
                    Console.WriteLine("Would you like to process payment now? (y/n)");

                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        await ProcessPaymentForOrderAsync(selectedOrder.OrderId);
                    }
                    else
                    {
                        Console.WriteLine("\nCannot mark order as Completed until payment is processed.");
                        return;
                    }
                }

                // If order has a table, mark it as not occupied
                if (selectedOrder.TableId.HasValue)
                {
                    var paymentTable = await _context.Tables.FindAsync(selectedOrder.TableId.Value);
                    if (paymentTable != null)
                    {
                        paymentTable.IsOccupied = false;
                    }
                }
            }

            // Special handling for Cancelled status
            if (newStatus == OrderStatus.Cancelled)
            {
                // If order has a table, mark it as not occupied
                if (selectedOrder.TableId.HasValue)
                {
                    var orderTable = await _context.Tables.FindAsync(selectedOrder.TableId.Value);
                    if (orderTable != null)
                    {
                        orderTable.IsOccupied = false;
                    }
                }

                // Handle paid orders that are being cancelled
                if (selectedOrder.IsPaid)
                {
                    Console.WriteLine("\nWarning: This order has already been paid.");
                    Console.WriteLine("Do you want to issue a refund? (y/n)");

                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        selectedOrder.Notes = (selectedOrder.Notes ?? "") + $"\nRefund issued on {DateTime.Now:g}";
                        selectedOrder.IsPaid = false;
                        selectedOrder.PaidDate = null;
                    }
                }
            }

            selectedOrder.Status = newStatus;
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nOrder #{selectedOrder.OrderId} status updated to {newStatus} successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
        }
    }

    private async Task ProcessPaymentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Process Payment ===");

        try
        {
            var unpaidOrders = await _context.Orders
                .Where(o => !o.IsPaid && (o.Status == OrderStatus.InProgress || o.Status == OrderStatus.Pending))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (unpaidOrders.Count == 0)
            {
                Console.WriteLine("No unpaid orders found.");
                return;
            }

            Console.WriteLine("\nUnpaid Orders:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15}", "ID", "Date", "Amount", "Status");
            Console.WriteLine(new string('-', 55));

            foreach (var order in unpaidOrders)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15}", 
                    order.OrderId, 
                    order.OrderDate.ToString("g"), 
                    order.TotalAmount, 
                    order.Status);
            }

            Console.Write("\nSelect Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId) ||
                !unpaidOrders.Any(o => o.OrderId == orderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            await ProcessPaymentForOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing payment: {ex.Message}");
        }
    }

    private async Task ProcessPaymentForOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        if (order.IsPaid)
        {
            Console.WriteLine("This order has already been paid.");
            return;
        }

        Console.WriteLine($"\nProcessing Payment for Order #{order.OrderId}");
        Console.WriteLine($"Total Amount: {order.TotalAmount:C}");

        Console.WriteLine("\nAvailable Payment Methods:");
        foreach (PaymentMethod method in Enum.GetValues(typeof(PaymentMethod)))
        {
            Console.WriteLine($"{(int)method}. {method}");
        }

        Console.Write("\nSelect payment method (number): ");
        if (!int.TryParse(Console.ReadLine(), out int methodValue) ||
            !Enum.IsDefined(typeof(PaymentMethod), methodValue))
        {
            Console.WriteLine("Invalid payment method selected.");
            return;
        }

        order.PaymentMethod = (PaymentMethod)methodValue;

        Console.Write("\nPayment Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount < 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        if (amount < order.TotalAmount)
        {
            Console.WriteLine("Warning: Payment amount is less than the order total.");
            Console.WriteLine("Continue anyway? (y/n)");

            if (Console.ReadLine()?.ToLower() != "y")
            {
                return;
            }

            // Record partial payment in notes
            order.Notes = (order.Notes ?? "") + $"\nPartial payment of {amount:C} received on {DateTime.Now:g}.";
        }
        else if (amount > order.TotalAmount)
        {
            decimal change = amount - order.TotalAmount;
            Console.WriteLine($"\nChange to return: {change:C}");

            // Record change given in notes
            order.Notes = (order.Notes ?? "") + $"\nChange given: {change:C}";
        }

        order.IsPaid = true;
        order.PaidDate = DateTime.Now;

        await _context.SaveChangesAsync();

        Console.WriteLine($"\nPayment for Order #{order.OrderId} processed successfully!");

        // Ask if order should be marked as completed
        if (order.Status != OrderStatus.Completed)
        {
            Console.WriteLine("\nWould you like to mark this order as Completed? (y/n)");

            if (Console.ReadLine()?.ToLower() == "y")
            {
                order.Status = OrderStatus.Completed;

                // If order has a table, mark it as not occupied
                if (order.TableId.HasValue)
                {
                    var table = await _context.Tables.FindAsync(order.TableId.Value);
                    if (table != null)
                    {
                        table.IsOccupied = false;
                    }
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"Order #{order.OrderId} marked as Completed.");
            }
        }
    }

    private async Task ViewOrderDetailsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View Order Details ===");

        try
        {
            // Display all orders
            await ViewAllOrdersAsync();

            Console.Write("\nEnter Order ID to view details: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedOrderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            var order = await _context.Orders
                .Include(o => o.Worker)
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == selectedOrderId);

            if (order == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== Order #{order.OrderId} Details ===");
            Console.WriteLine($"Date: {order.OrderDate:g}");
            Console.WriteLine($"Status: {order.Status}");
            Console.WriteLine($"Worker: {order.Worker?.Name ?? "Unknown"}");
            Console.WriteLine($"Table: {(order.Table != null ? $"Table {order.Table.TableNumber}" : "Take-away")}");
            Console.WriteLine($"Payment Method: {order.PaymentMethod}");
            Console.WriteLine($"Paid: {(order.IsPaid ? $"Yes (on {order.PaidDate:g})" : "No")}");

            if (!string.IsNullOrWhiteSpace(order.Notes))
            {
                Console.WriteLine($"Notes: {order.Notes}");
            }

            Console.WriteLine("\nOrder Items:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10} {4,-10}", 
                "ID", "Item", "Quantity", "Unit Price", "Total");
            Console.WriteLine(new string('-', 60));

            if (order.OrderItems.Count == 0)
            {
                Console.WriteLine("No items in this order.");
            }
            else
            {
                foreach (var item in order.OrderItems)
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10:C} {4,-10:C}", 
                        item.OrderItemId, 
                        item.Item?.Name ?? "Unknown", 
                        item.Quantity, 
                        item.UnitPrice, 
                        item.TotalPrice);
                }

                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error viewing order details: {ex.Message}");
        }
    }

    private async Task CancelOrderAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Cancel Order ===");

        try
        {
            var activeOrders = await _context.Orders
                .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (activeOrders.Count == 0)
            {
                Console.WriteLine("No active orders found.");
                return;
            }

            Console.WriteLine("\nActive Orders:");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-15}", "ID", "Date", "Amount", "Status");
            Console.WriteLine(new string('-', 55));

            foreach (var order in activeOrders)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10:C} {3,-15}", 
                    order.OrderId, 
                    order.OrderDate.ToString("g"), 
                    order.TotalAmount, 
                    order.Status);
            }

            Console.Write("\nSelect Order ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId) ||
                !activeOrders.Any(o => o.OrderId == orderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            var selectedOrder = activeOrders.First(o => o.OrderId == orderId);

            Console.Write($"\nAre you sure you want to cancel Order #{selectedOrder.OrderId}? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("\nCancellation aborted.");
                return;
            }

            Console.Write("\nReason for cancellation: ");
            string? reason = Console.ReadLine();

            selectedOrder.Status = OrderStatus.Cancelled;
            selectedOrder.Notes = (selectedOrder.Notes ?? "") + $"\nCancelled on {DateTime.Now:g}" + 
                           (!string.IsNullOrWhiteSpace(reason) ? $": {reason}" : "");

            // If order has a table, mark it as not occupied
            if (selectedOrder.TableId.HasValue)
            {
                var orderTable = await _context.Tables.FindAsync(selectedOrder.TableId.Value);
                if (orderTable != null)
                {
                    orderTable.IsOccupied = false;
                }
            }

            // Handle paid orders that are being cancelled
            if (selectedOrder.IsPaid)
            {
                Console.WriteLine("\nWarning: This order has already been paid.");
                Console.WriteLine("Do you want to issue a refund? (y/n)");

                if (Console.ReadLine()?.ToLower() == "y")
                {
                    selectedOrder.Notes = (selectedOrder.Notes ?? "") + $"\nRefund issued on {DateTime.Now:g}";
                    selectedOrder.IsPaid = false;
                    selectedOrder.PaidDate = null;
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"\nOrder #{selectedOrder.OrderId} cancelled successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling order: {ex.Message}");
        }
    }

    #endregion

    #region Worker Management

    private async Task ManageWorkersMenuAsync()
    {
        bool back = false;

        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== Manage Workers ===");
            Console.WriteLine("1. View All Workers");
            Console.WriteLine("2. Add New Worker");
            Console.WriteLine("3. Update Worker");
            Console.WriteLine("4. Change Worker Status");
            Console.WriteLine("5. Back to Main Menu");

            Console.Write("\nSelect an option (1-5): ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ViewAllWorkersAsync();
                    break;
                case "2":
                    await AddNewWorkerAsync();
                    break;
                case "3":
                    await UpdateWorkerAsync();
                    break;
                case "4":
                    await ChangeWorkerStatusAsync();
                    break;
                case "5":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!back)
            {
                Console.WriteLine("\nPress any key to return to the Workers menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task ViewAllWorkersAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Workers ===");

        var workers = await _context.Workers.ToListAsync();

        if (workers.Count == 0)
        {
            Console.WriteLine("No workers found.");
            return;
        }

        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-20}", 
            "ID", "Name", "Role", "Active", "Created Date");
        Console.WriteLine(new string('-', 75));

        foreach (var worker in workers)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-20}", 
                worker.WorkerId, 
                worker.Name, 
                worker.Role, 
                worker.IsActive ? "Yes" : "No", 
                worker.CreatedDate.ToString("g"));
        }
    }

    private async Task AddNewWorkerAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Worker ===");

        try
        {
            Console.Write("Worker Name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Worker name cannot be empty.");
                return;
            }

            Console.Write("Username: ");
            string? username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            // Check if username already exists
            bool usernameExists = await _context.Workers.AnyAsync(w => w.Username == username);

            if (usernameExists)
            {
                Console.WriteLine("This username already exists. Please choose another.");
                return;
            }

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty.");
                return;
            }

            // Display available roles
            Console.WriteLine("\nAvailable Worker Roles:");
            foreach (WorkerRole role in Enum.GetValues(typeof(WorkerRole)))
            {
                Console.WriteLine($"{(int)role}. {role}");
            }

            Console.Write("\nSelect Worker Role (number): ");
            if (!int.TryParse(Console.ReadLine(), out int roleValue) ||
                !Enum.IsDefined(typeof(WorkerRole), roleValue))
            {
                Console.WriteLine("Invalid role. Setting to Cashier by default.");
                roleValue = (int)WorkerRole.Cashier;
            }

            // Create new worker
            var newWorker = new Worker
            {
                Name = name,
                Username = username,
                Password = password, // In production, this should be hashed
                Role = (WorkerRole)roleValue,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Workers.Add(newWorker);
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nWorker '{name}' added successfully with ID: {newWorker.WorkerId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding worker: {ex.Message}");
        }
    }

    private async Task UpdateWorkerAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Worker ===");

        try
        {
            await ViewAllWorkersAsync();

            Console.Write("\nEnter Worker ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int workerId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var worker = await _context.Workers.FindAsync(workerId);

            if (worker == null)
            {
                Console.WriteLine("Worker not found.");
                return;
            }

            Console.WriteLine($"\nUpdating Worker: {worker.Name}");

            Console.Write($"New Name (current: {worker.Name}): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                worker.Name = name;
            }

            Console.Write($"New Username (current: {worker.Username}): ");
            string? username = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(username) && username != worker.Username)
            {
                // Check if username already exists
                bool usernameExists = await _context.Workers.AnyAsync(w => w.Username == username && w.WorkerId != workerId);

                if (usernameExists)
                {
                    Console.WriteLine("This username already exists. Username not updated.");
                }
                else
                {
                    worker.Username = username;
                }
            }

            Console.Write("New Password (leave empty to keep current): ");
            string? password = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(password))
            {
                worker.Password = password; // In production, this should be hashed
            }

            // Display available roles
            Console.WriteLine("\nAvailable Worker Roles:");
            foreach (WorkerRole role in Enum.GetValues(typeof(WorkerRole)))
            {
                Console.WriteLine($"{(int)role}. {role}");
            }

            Console.Write($"New Role (current: {worker.Role}): ");
            string? roleInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(roleInput) && int.TryParse(roleInput, out int roleValue) &&
                Enum.IsDefined(typeof(WorkerRole), roleValue))
            {
                worker.Role = (WorkerRole)roleValue;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("\nWorker updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating worker: {ex.Message}");
        }
    }

    private async Task ChangeWorkerStatusAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Change Worker Status ===");

        try
        {
            await ViewAllWorkersAsync();

            Console.Write("\nEnter Worker ID to change status: ");
            if (!int.TryParse(Console.ReadLine(), out int workerId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var worker = await _context.Workers.FindAsync(workerId);

            if (worker == null)
            {
                Console.WriteLine("Worker not found.");
                return;
            }

            Console.WriteLine($"\nCurrent Status of '{worker.Name}': {(worker.IsActive ? "Active" : "Inactive")}");
            Console.WriteLine($"Would you like to mark this worker as {(worker.IsActive ? "Inactive" : "Active")}? (y/n)");

            if (Console.ReadLine()?.ToLower() == "y")
            {
                // Check if this is the last active admin/manager
                if (worker.IsActive && 
                    (worker.Role == WorkerRole.Admin || worker.Role == WorkerRole.Manager))
                {
                    int activeAdminOrManagerCount = await _context.Workers
                        .CountAsync(w => w.IsActive && 
                                  (w.Role == WorkerRole.Admin || w.Role == WorkerRole.Manager) && 
                                  w.WorkerId != workerId);

                    if (activeAdminOrManagerCount == 0)
                    {
                        Console.WriteLine("\nCannot deactivate the last active Admin/Manager.");
                        Console.WriteLine("Please add another Admin/Manager first.");
                        return;
                    }
                }

                worker.IsActive = !worker.IsActive;
                await _context.SaveChangesAsync();

                Console.WriteLine($"\nWorker '{worker.Name}' is now {(worker.IsActive ? "Active" : "Inactive")}.");
            }
            else
            {
                Console.WriteLine("\nStatus change cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing worker status: {ex.Message}");
        }
    }

    #endregion

    #region Reports

    private async Task ViewReportsMenuAsync()
    {
        bool back = false;

        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== View Reports ===");
            Console.WriteLine("1. Sales Report");
            Console.WriteLine("2. Popular Items Report");
            Console.WriteLine("3. Worker Performance Report");
            Console.WriteLine("4. Daily Sales Report");
            Console.WriteLine("5. Inventory Status Report");
            Console.WriteLine("6. Back to Main Menu");

            Console.Write("\nSelect an option (1-6): ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await SalesReportAsync();
                    break;
                case "2":
                    await PopularItemsReportAsync();
                    break;
                case "3":
                    await WorkerPerformanceReportAsync();
                    break;
                case "4":
                    await DailySalesReportAsync();
                    break;
                case "5":
                    await InventoryStatusReportAsync();
                    break;
                case "6":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!back)
            {
                Console.WriteLine("\nPress any key to return to the Reports menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task SalesReportAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Sales Report ===");

        try
        {
            Console.WriteLine("Select time period:");
            Console.WriteLine("1. Today");
            Console.WriteLine("2. This Week");
            Console.WriteLine("3. This Month");
            Console.WriteLine("4. Custom Date Range");

            Console.Write("\nSelect an option (1-4): ");
            string? option = Console.ReadLine();

            DateTime startDate, endDate;
            endDate = DateTime.Now.Date.AddDays(1); // End date is tomorrow at midnight

            switch (option)
            {
                case "1": // Today
                    startDate = DateTime.Now.Date;
                    break;
                case "2": // This Week
                    startDate = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                    break;
                case "3": // This Month
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                case "4": // Custom Date Range
                    Console.Write("\nEnter start date (MM/DD/YYYY): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out startDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        return;
                    }

                    Console.Write("Enter end date (MM/DD/YYYY): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        return;
                    }
                    endDate = endDate.Date.AddDays(1); // Include the end date fully
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }

            Console.Clear();
            Console.WriteLine($"=== Sales Report: {startDate:d} to {endDate.AddDays(-1):d} ===");

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .ToListAsync();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found in the selected period.");
                return;
            }

            decimal totalSales = orders.Sum(o => o.TotalAmount);
            int completedOrders = orders.Count(o => o.Status == OrderStatus.Completed);
            int cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);
            int pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress);

            Console.WriteLine($"Total Sales: {totalSales:C}");
            Console.WriteLine($"Number of Orders: {orders.Count}");
            Console.WriteLine($"Completed Orders: {completedOrders}");
            Console.WriteLine($"Cancelled Orders: {cancelledOrders}");
            Console.WriteLine($"Pending/In-Progress Orders: {pendingOrders}");

            if (completedOrders > 0)
            {
                decimal avgOrderValue = totalSales / completedOrders;
                Console.WriteLine($"Average Order Value: {avgOrderValue:C}");
            }

            // Payment method breakdown
            Console.WriteLine("\nPayment Method Breakdown:");
            var paymentMethods = orders
                .Where(o => o.IsPaid)
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(o => o.TotalAmount) })
                .OrderByDescending(x => x.Total);

            foreach (var method in paymentMethods)
            {
                Console.WriteLine($"{method.Method}: {method.Count} orders, {method.Total:C} ({method.Total / totalSales:P1})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating sales report: {ex.Message}");
        }
    }

    private async Task PopularItemsReportAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Popular Items Report ===");

        try
        {
            Console.WriteLine("Select time period:");
            Console.WriteLine("1. Today");
            Console.WriteLine("2. This Week");
            Console.WriteLine("3. This Month");
            Console.WriteLine("4. All Time");

            Console.Write("\nSelect an option (1-4): ");
            string? option = Console.ReadLine();

            DateTime? startDate = null;

            switch (option)
            {
                case "1": // Today
                    startDate = DateTime.Now.Date;
                    break;
                case "2": // This Week
                    startDate = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                    break;
                case "3": // This Month
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                case "4": // All Time
                    startDate = null;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }

            Console.Clear();
            string title = startDate.HasValue ? $"=== Popular Items: {startDate.Value:d} to {DateTime.Now:d} ===" : "=== Popular Items: All Time ===";
            Console.WriteLine(title);

            var query = _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Item)
                    .ThenInclude(i => i.Category)
                .Where(oi => oi.Order.Status != OrderStatus.Cancelled);

            if (startDate.HasValue)
            {
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);
            }

            var itemStats = await query
                .GroupBy(oi => new { oi.ItemId, ItemName = oi.Item.Name, CategoryName = oi.Item.Category.Name })
                .Select(g => new
                {
                    g.Key.ItemId,
                    g.Key.ItemName,
                    g.Key.CategoryName,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalSales = g.Sum(oi => oi.TotalPrice),
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            if (itemStats.Count == 0)
            {
                Console.WriteLine("No order data found in the selected period.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-15} {5,-10}", 
                "Rank", "Item", "Category", "Quantity", "Sales", "Orders");
            Console.WriteLine(new string('-', 80));

            int rank = 1;
            foreach (var stat in itemStats)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-15:C} {5,-10}", 
                    rank++, 
                    stat.ItemName, 
                    stat.CategoryName, 
                    stat.TotalQuantity, 
                    stat.TotalSales, 
                    stat.OrderCount);
            }

            // Category analysis
            Console.WriteLine("\nCategory Performance:");
            var categoryStats = await query
                .GroupBy(oi => oi.Item.Category.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(oi => oi.TotalPrice),
                    ItemCount = g.Select(oi => oi.ItemId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            foreach (var stat in categoryStats)
            {
                Console.WriteLine($"{stat.CategoryName}: {stat.TotalSales:C} ({stat.ItemCount} items)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating popular items report: {ex.Message}");
        }
    }

    private async Task WorkerPerformanceReportAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Worker Performance Report ===");

        try
        {
            Console.WriteLine("Select time period:");
            Console.WriteLine("1. Today");
            Console.WriteLine("2. This Week");
            Console.WriteLine("3. This Month");
            Console.WriteLine("4. All Time");

            Console.Write("\nSelect an option (1-4): ");
            string? option = Console.ReadLine();

            DateTime? startDate = null;

            switch (option)
            {
                case "1": // Today
                    startDate = DateTime.Now.Date;
                    break;
                case "2": // This Week
                    startDate = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                    break;
                case "3": // This Month
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                case "4": // All Time
                    startDate = null;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }

            Console.Clear();
            string title = startDate.HasValue ? $"=== Worker Performance: {startDate.Value:d} to {DateTime.Now:d} ===" : "=== Worker Performance: All Time ===";
            Console.WriteLine(title);

            IQueryable<Order> query = _context.Orders;

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            query = query.Include(o => o.Worker);

            var workerStats = await query
                .GroupBy(o => new { o.WorkerId, WorkerName = o.Worker.Name, o.Worker.Role })
                .Select(g => new
                {
                    g.Key.WorkerId,
                    g.Key.WorkerName,
                    g.Key.Role,
                    TotalOrders = g.Count(),
                    CompletedOrders = g.Count(o => o.Status == OrderStatus.Completed),
                    CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled),
                    TotalSales = g.Sum(o => o.TotalAmount),
                    AvgOrderValue = g.Count() > 0 ? g.Average(o => o.TotalAmount) : 0
                })
                .OrderByDescending(w => w.TotalSales)
                .ToListAsync();

            if (workerStats.Count == 0)
            {
                Console.WriteLine("No order data found in the selected period.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-15} {4,-15} {5,-10}", 
                "ID", "Name", "Role", "Orders", "Sales", "Avg Order");
            Console.WriteLine(new string('-', 85));

            foreach (var stat in workerStats)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-15} {4,-15:C} {5,-10:C}", 
                    stat.WorkerId, 
                    stat.WorkerName, 
                    stat.Role, 
                    $"{stat.CompletedOrders}/{stat.TotalOrders}", 
                    stat.TotalSales, 
                    stat.AvgOrderValue);
            }

            // Best performing worker
            if (workerStats.Count > 0)
            {
                var bestWorker = workerStats.OrderByDescending(w => w.TotalSales).First();
                Console.WriteLine($"\nBest Performing Worker: {bestWorker.WorkerName} ({bestWorker.TotalSales:C})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating worker performance report: {ex.Message}");
        }
    }

    private async Task DailySalesReportAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Daily Sales Report ===");

        try
        {
            Console.WriteLine("Select period:");
            Console.WriteLine("1. Last 7 Days");
            Console.WriteLine("2. Last 30 Days");
            Console.WriteLine("3. Current Month");
            Console.WriteLine("4. Custom Date Range");

            Console.Write("\nSelect an option (1-4): ");
            string? option = Console.ReadLine();

            DateTime startDate, endDate;
            endDate = DateTime.Now.Date.AddDays(1); // End date is tomorrow at midnight

            switch (option)
            {
                case "1": // Last 7 Days
                    startDate = DateTime.Now.Date.AddDays(-7);
                    break;
                case "2": // Last 30 Days
                    startDate = DateTime.Now.Date.AddDays(-30);
                    break;
                case "3": // Current Month
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                case "4": // Custom Date Range
                    Console.Write("\nEnter start date (MM/DD/YYYY): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out startDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        return;
                    }

                    Console.Write("Enter end date (MM/DD/YYYY): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        return;
                    }
                    endDate = endDate.Date.AddDays(1); // Include the end date fully
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }

            Console.Clear();
            Console.WriteLine($"=== Daily Sales: {startDate:d} to {endDate.AddDays(-1):d} ===");

            var dailySales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalSales = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    CompletedOrders = g.Count(o => o.Status == OrderStatus.Completed),
                    CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            if (dailySales.Count == 0)
            {
                Console.WriteLine("No sales data found in the selected period.");
                return;
            }

            Console.WriteLine("{0,-12} {1,-15} {2,-15} {3,-15} {4,-15}", 
                "Date", "Sales", "Orders", "Completed", "Cancelled");
            Console.WriteLine(new string('-', 75));

            decimal totalSales = 0;
            int totalOrders = 0;

            foreach (var day in dailySales)
            {
                Console.WriteLine("{0,-12} {1,-15:C} {2,-15} {3,-15} {4,-15}", 
                    day.Date.ToString("MM/dd/yyyy"), 
                    day.TotalSales, 
                    day.OrderCount, 
                    day.CompletedOrders, 
                    day.CancelledOrders);

                totalSales += day.TotalSales;
                totalOrders += day.OrderCount;
            }

            Console.WriteLine(new string('-', 75));
            Console.WriteLine("{0,-12} {1,-15:C} {2,-15}", 
                "Total:", totalSales, totalOrders);

            // Find best and worst day
            if (dailySales.Count > 0)
            {
                var bestDay = dailySales.OrderByDescending(d => d.TotalSales).First();
                var worstDay = dailySales.OrderBy(d => d.TotalSales).First();

                Console.WriteLine($"\nBest Day: {bestDay.Date:d} ({bestDay.TotalSales:C}, {bestDay.OrderCount} orders)");
                Console.WriteLine($"Worst Day: {worstDay.Date:d} ({worstDay.TotalSales:C}, {worstDay.OrderCount} orders)");

                // Average daily sales
                decimal avgDailySales = totalSales / dailySales.Count;
                Console.WriteLine($"Average Daily Sales: {avgDailySales:C}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating daily sales report: {ex.Message}");
        }
    }

    private async Task InventoryStatusReportAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Inventory Status Report ===");

        try
        {
            // Get items by status
            var itemsByStatus = await _context.Items
                .GroupBy(i => i.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderBy(s => s.Status)
                .ToListAsync();

            Console.WriteLine("\nItem Status Summary:");
            foreach (var status in itemsByStatus)
            {
                Console.WriteLine($"{status.Status}: {status.Count} items");
            }

            // Get items by category
            var itemsByCategory = await _context.Items
                .Include(i => i.Category)
                .GroupBy(i => i.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalItems = g.Count(),
                    AvailableItems = g.Count(i => i.Status == ItemStatus.Available),
                    OutOfStockItems = g.Count(i => i.Status == ItemStatus.OutOfStock),
                    DiscontinuedItems = g.Count(i => i.Status == ItemStatus.Discontinued)
                })
                .OrderBy(c => c.Category)
                .ToListAsync();

            Console.WriteLine("\nItems by Category:");
            Console.WriteLine("{0,-20} {1,-15} {2,-15} {3,-15} {4,-15}", 
                "Category", "Total Items", "Available", "Out of Stock", "Discontinued");
            Console.WriteLine(new string('-', 80));

            foreach (var category in itemsByCategory)
            {
                Console.WriteLine("{0,-20} {1,-15} {2,-15} {3,-15} {4,-15}", 
                    category.Category, 
                    category.TotalItems, 
                    category.AvailableItems, 
                    category.OutOfStockItems, 
                    category.DiscontinuedItems);
            }

            // Out of stock items
            var outOfStockItems = await _context.Items
                .Where(i => i.Status == ItemStatus.OutOfStock)
                .Include(i => i.Category)
                .OrderBy(i => i.Name)
                .ToListAsync();

            if (outOfStockItems.Count > 0)
            {
                Console.WriteLine("\nOut of Stock Items:");
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10}", 
                    "ID", "Name", "Category", "Price");
                Console.WriteLine(new string('-', 55));

                foreach (var item in outOfStockItems)
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10:C}", 
                        item.ItemId, 
                        item.Name, 
                        item.Category?.Name ?? "No Category", 
                        item.Price);
                }
            }

            // Popular items that are out of stock
            var popularOutOfStockItems = await _context.OrderItems
                .Include(oi => oi.Item)
                .Where(oi => oi.Item.Status == ItemStatus.OutOfStock)
                .GroupBy(oi => new { oi.ItemId, ItemName = oi.Item.Name })
                .Select(g => new
                {
                    g.Key.ItemId,
                    g.Key.ItemName,
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count(),
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(i => i.TotalQuantity)
                .Take(5)
                .ToListAsync();

            if (popularOutOfStockItems.Count > 0)
            {
                Console.WriteLine("\nPopular Out of Stock Items (Restock Priority):");
                foreach (var item in popularOutOfStockItems)
                {
                    Console.WriteLine($"{item.ItemName} (ID: {item.ItemId}) - Ordered {item.TotalQuantity} times in {item.OrderCount} orders");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating inventory status report: {ex.Message}");
        }
    }

    #endregion
}