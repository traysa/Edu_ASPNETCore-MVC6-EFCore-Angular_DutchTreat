using System.ComponentModel.DataAnnotations;

namespace DutchTreat.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; } // Auto incremented ID
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        // Because the View Models will be hiearchical, the folling is not neeeded
        //public Order Order { get; set; } // Relation to entity "Order"
        //public Product Product { get; set; } // Relation to entity "Product"
        
        // Flatening the products into this view model
        // No mapping in DutchMappingProfile.cs needed. Properties just need to be prefixed with the entity they are referring to.
        // The auto mapper automatically resolves the sub-object "product"
        [Required]
        public int ProductId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSize { get; set; }
        public string ProductTitle { get; set; }        
        public string ProductArtist { get; set; }
        public string ProductArtId { get; set; } // Needed for image links
    }
}