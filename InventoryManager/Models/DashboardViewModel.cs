namespace InventoryManager.Models
{
    public class DashboardViewModel
    {
        // Stat cards
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }

        // Chart data
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}