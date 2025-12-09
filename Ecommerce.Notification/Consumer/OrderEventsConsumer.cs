using BuildingBlocks.Contracts.Messages.Events;
using MassTransit;

namespace Ecommerce.Notification.Consumer
{

    public class OrderEventsConsumer :
        IConsumer<OrderCreatedEvent>,
        IConsumer<OrderCompletedEvent>,
        IConsumer<OrderCancelledEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine($"[Notification] Order created: {context.Message.OrderId}, Amount: {context.Message.TotalAmount}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            Console.WriteLine($"[Notification] Order completed: {context.Message.OrderId}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<OrderCancelledEvent> context)
        {
            Console.WriteLine($"[Notification] Order cancelled: {context.Message.OrderId}, Reason: {context.Message.Reason}");
            return Task.CompletedTask;
        }
    }
}
