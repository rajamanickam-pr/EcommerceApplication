using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommerce.Order.Domain;
using Ecommerce.Order.Infrastructure;
using MassTransit;

namespace Ecommerce.Order.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly OrderDbContext _db;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public StockReservedEventConsumer(OrderDbContext db, ISendEndpointProvider sendEndpointProvider)
        {
            _db = db;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var order = await _db.Orders.FindAsync(context.Message.OrderId);
            if (order is null) return;

            order.Status = OrderStatus.StockReserved;
            await _db.SaveChangesAsync();

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:payment-authorize"));
            await endpoint.Send(new AuthorizePaymentCommand(order.Id, order.TotalAmount));
        }
    }
}
