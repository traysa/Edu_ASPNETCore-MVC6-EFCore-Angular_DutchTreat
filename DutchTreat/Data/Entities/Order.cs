using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Data.Entities
{
  public class Order
  {
    public int Id { get; set; } // Auto incremented ID
    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; }

    // With a collection a realtion between entities can be built;
    // Parent-Child (1-to-many) Relationship: One Order can have many order items
    public ICollection<OrderItem> Items { get; set; }

    // Store order user
    public StoreUser User { get; set; }
  }
}
