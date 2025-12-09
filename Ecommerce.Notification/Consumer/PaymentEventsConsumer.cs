using BuildingBlocks.Contracts.Messages.Events;
using MassTransit;

namespace Ecommerce.Notification.Consumer
{

    public class PaymentEventsConsumer :
        IConsumer<PaymentAuthorizedEvent>,
        IConsumer<PaymentFailedEvent>
    {
        public Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
        {
            Console.WriteLine($"[Notification] Payment authorized for Order {context.Message.OrderId}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Console.WriteLine($"[Notification] Payment failed for Order {context.Message.OrderId}: {context.Message.Reason}");
            return Task.CompletedTask;
        }
    }
}
