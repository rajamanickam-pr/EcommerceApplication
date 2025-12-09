namespace Ecommerce.Order.Infrastructure
{
    public static class OrderDbInitializer
    {
        public static async Task InitializeAsync(OrderDbContext db)
        {
            await db.Database.EnsureCreatedAsync();

            // No seed required for orders – they will be created via API.
            // You could seed test orders here if you really want.
        }
    }
}
