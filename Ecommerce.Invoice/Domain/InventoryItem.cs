namespace Ecommerce.Inventory.Domain
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
