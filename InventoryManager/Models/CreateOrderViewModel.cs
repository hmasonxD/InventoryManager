using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }
    }

    public class CreateOrderViewModel
    {
        public string? Notes { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}