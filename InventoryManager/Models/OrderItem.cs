using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        // Foreign keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantity * UnitPrice;
    }
}