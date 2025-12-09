using Ecommerce.Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Inventory.Infrastructure
{
    public static class InventoryDbInitializer
    {
        public static async Task InitializeAsync(InventoryDbContext db)
        {
            // Creates DB and tables if they do not exist
            await db.Database.EnsureCreatedAsync();

            // Seed only if empty
            if (!await db.InventoryItems.AnyAsync())
            {
                var items = new List<InventoryItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    AvailableQuantity = 100,
                    ReservedQuantity = 0
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    AvailableQuantity = 10,
                    ReservedQuantity = 0
                }
            };

                db.InventoryItems.AddRange(items);
                await db.SaveChangesAsync();
            }
        }
    }
}
