using BuildingBlocks.Contracts.Messages.Commands;
using Ecommerce.Inventory.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Inventory.Consumers
{
    public class ReleaseStockCommandConsumer : IConsumer<ReleaseStockCommand>
    {
        private readonly InventoryDbContext _db;

        public ReleaseStockCommandConsumer(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<ReleaseStockCommand> context)
        {
            foreach (var item in context.Message.Items)
            {
                var inv = await _db.InventoryItems
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                if (inv is null) continue;

                inv.ReservedQuantity -= item.Quantity;
                inv.AvailableQuantity += item.Quantity;
            }

            await _db.SaveChangesAsync();
        }
    }
}
