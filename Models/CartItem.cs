using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shofy.Models
{
    public class CartItem
    {
        [Key]
        public int ItemID { get; set; }

        [ForeignKey("Cart")]
        public int CartID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0.01, double.MaxValue), Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
