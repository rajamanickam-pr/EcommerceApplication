using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommerce.Order.Domain;
using Ecommerce.Order.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Order.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly OrderDbContext _db;
        private readonly IPublishEndpoint _publish;
        private readonly ISendEndpointProvider _send;

        public PaymentFailedEventConsumer(OrderDbContext db, IPublishEndpoint publish, ISendEndpointProvider send)
        {
            _db = db;
            _publish = publish;
            _send = send;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

            if (order is null) return;

            order.Status = OrderStatus.Cancelled;
            await _db.SaveChangesAsync();

            // compensation: release stock
            var endpoint = await _send.GetSendEndpoint(new Uri("queue:inventory-release-stock"));
            await endpoint.Send(new ReleaseStockCommand(
                order.Id,
                order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.Price)).ToList()
            ));

            await _publish.Publish(new OrderCancelledEvent(order.Id, context.Message.Reason));
        }
    }
}
