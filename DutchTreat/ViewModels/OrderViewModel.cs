using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.ViewModels
{
    public class OrderViewModel
    {
        // Using data annotations for validation of the userinput
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        [MinLength(4)]
        public string OrderNumber { get; set; }

        // To also auto. support returning Items data via the AutoMapper,
        // we had a collection of an Order Item View Model
        public ICollection<OrderItemViewModel> Items { get; set; }
    }
}
