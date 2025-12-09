namespace Ecommerce.Order.Domain
{
    public enum OrderStatus
    {
        Pending,
        StockReserved,
        PaymentAuthorized,
        Completed,
        Cancelled
    }
}
