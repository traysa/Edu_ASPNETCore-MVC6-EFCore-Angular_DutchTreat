namespace DutchTreat.Data.Entities
{
  public class OrderItem
  {
    public int Id { get; set; } // Auto incremented ID
    public Product Product { get; set; } // Relation to entity "Product"
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Order Order { get; set; } // Relation to entity "Order"
    }
}