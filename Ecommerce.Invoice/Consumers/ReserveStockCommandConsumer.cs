using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommerce.Inventory.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Inventory.Consumers
{
    public class ReserveStockCommandConsumer : IConsumer<ReserveStockCommand>
    {
        private readonly InventoryDbContext _db;

        public ReserveStockCommandConsumer(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<ReserveStockCommand> context)
        {
            var items = context.Message.Items;
            foreach (var item in items)
            {
                var inv = await _db.InventoryItems
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                if (inv is null || inv.AvailableQuantity < item.Quantity)
                {
                    await context.Publish(new StockReservationFailedEvent(
                        context.Message.OrderId,
                        $"Insufficient stock for product {item.ProductId}"
                    ));
                    return;
                }
            }

            // All good: deduct
            foreach (var item in items)
            {
                var inv = await _db.InventoryItems
                    .FirstAsync(i => i.ProductId == item.ProductId);

                inv.AvailableQuantity -= item.Quantity;
                inv.ReservedQuantity += item.Quantity;
            }

            await _db.SaveChangesAsync();

            await context.Publish(new StockReservedEvent(context.Message.OrderId));
        }
    }
}
