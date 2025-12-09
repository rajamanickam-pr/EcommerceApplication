using Microsoft.EntityFrameworkCore;
using Ecommere.Payment.Domain;

namespace Ecommere.Payment.Infrastructure
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<Payments> Payments => Set<Payments>();
    }
}
