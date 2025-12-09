using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommere.Payment.Domain;
using Ecommere.Payment.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommere.Payment.Consumers
{
    public class RefundPaymentCommandConsumer : IConsumer<RefundPaymentCommand>
    {
        private readonly PaymentDbContext _db;

        public RefundPaymentCommandConsumer(PaymentDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.OrderId == context.Message.OrderId);

            if (payment is null) return;

            payment.Status = PaymentStatus.Refunded;
            await _db.SaveChangesAsync();

            await context.Publish(new PaymentRefundedEvent(payment.OrderId));
        }
    }
}
