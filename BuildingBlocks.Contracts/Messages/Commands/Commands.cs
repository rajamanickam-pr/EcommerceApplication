namespace BuildingBlocks.Contracts.Messages.Commands
{
    public record ReserveStockCommand(
    Guid OrderId,
    IReadOnlyCollection<OrderItemDto> Items);

    public record ReleaseStockCommand(
        Guid OrderId,
        IReadOnlyCollection<OrderItemDto> Items);

    public record AuthorizePaymentCommand(
        Guid OrderId,
        decimal Amount);

    public record RefundPaymentCommand(
        Guid OrderId,
        decimal Amount);

    public record OrderItemDto(Guid ProductId, int Quantity, decimal Price);
}
