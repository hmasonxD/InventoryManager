using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Low Stock Threshold")]
        public int LowStockThreshold { get; set; } = 10;

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        // Foreign keys
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }

        // Computed property — not stored in DB
        [NotMapped]
        public bool IsLowStock => Quantity <= LowStockThreshold;
    }
}