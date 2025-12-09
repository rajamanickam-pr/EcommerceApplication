using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommere.Payment.Domain;
using Ecommere.Payment.Infrastructure;
using MassTransit;

namespace Ecommere.Payment.Consumers
{

    public class AuthorizePaymentCommandConsumer : IConsumer<AuthorizePaymentCommand>
    {
        private readonly PaymentDbContext _db;

        public AuthorizePaymentCommandConsumer(PaymentDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<AuthorizePaymentCommand> context)
        {
            var payment = new Payments
            {
                Id = Guid.NewGuid(),
                OrderId = context.Message.OrderId,
                Amount = context.Message.Amount,
                CreatedAt = DateTime.UtcNow,
                Status = PaymentStatus.Pending
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            // Simulate random failures (e.g. 20%)
            var rnd = Random.Shared.Next(1, 101);
            if (rnd <= 20)
            {
                payment.Status = PaymentStatus.Failed;
                await _db.SaveChangesAsync();

                await context.Publish(new PaymentFailedEvent(payment.OrderId, "Simulated payment failure"));
                return;
            }

            payment.Status = PaymentStatus.Authorized;
            await _db.SaveChangesAsync();

            await context.Publish(new PaymentAuthorizedEvent(payment.OrderId));
        }
    }
}
