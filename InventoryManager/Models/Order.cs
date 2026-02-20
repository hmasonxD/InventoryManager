using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Notes { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [NotMapped]
        [Display(Name = "Total")]
        public decimal TotalAmount => OrderItems.Sum(oi => oi.Subtotal);
    }
}