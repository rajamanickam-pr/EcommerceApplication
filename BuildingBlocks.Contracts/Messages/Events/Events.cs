namespace BuildingBlocks.Contracts.Messages.Events
{
    //Inventory events
    public record StockReservedEvent(Guid OrderId);
    public record StockReservationFailedEvent(Guid OrderId, string Reason);

    //Payment events
    public record PaymentAuthorizedEvent(Guid OrderId);
    public record PaymentFailedEvent(Guid OrderId, string Reason);
    public record PaymentRefundedEvent(Guid OrderId);

    //Order Events
    public record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal TotalAmount);
    public record OrderCompletedEvent(Guid OrderId);
    public record OrderCancelledEvent(Guid OrderId, string Reason);
}
