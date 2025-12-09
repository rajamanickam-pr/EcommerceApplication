using BuildingBlocks.Contracts.Messages.Events;
using Ecommerce.Order.Domain;
using Ecommerce.Order.Infrastructure;
using MassTransit;

namespace Ecommerce.Order.Consumers
{
    public class StockReservationFailedEventConsumer : IConsumer<StockReservationFailedEvent>
    {
        private readonly OrderDbContext _db;
        private readonly IPublishEndpoint _publish;

        public StockReservationFailedEventConsumer(OrderDbContext db, IPublishEndpoint publish)
        {
            _db = db;
            _publish = publish;
        }

        public async Task Consume(ConsumeContext<StockReservationFailedEvent> context)
        {
            var order = await _db.Orders.FindAsync(context.Message.OrderId);
            if (order is null) return;

            order.Status = OrderStatus.Cancelled;
            await _db.SaveChangesAsync();

            await _publish.Publish(new OrderCancelledEvent(order.Id, context.Message.Reason));
        }
    }
}
