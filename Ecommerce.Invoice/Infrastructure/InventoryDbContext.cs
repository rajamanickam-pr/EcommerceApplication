using Ecommerce.Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Inventory.Infrastructure
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    }
}
