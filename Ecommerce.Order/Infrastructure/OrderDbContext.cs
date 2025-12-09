using Ecommerce.Order.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Order.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
