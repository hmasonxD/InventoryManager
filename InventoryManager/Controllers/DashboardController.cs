using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor injection — ASP.NET automatically provides the database context
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Build the ViewModel by querying the database
            var viewModel = new DashboardViewModel
            {
                // Count totals from each table
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),

                // Find products where quantity is at or below threshold
                LowStockCount = await _context.Products
                    .Where(p => p.Quantity <= p.LowStockThreshold)
                    .CountAsync(),

                // Sum up (Price * Quantity) for every product
                TotalInventoryValue = await _context.Products
                    .SumAsync(p => p.Price * p.Quantity),

                // Include Products so we can count per category for the chart
                Categories = await _context.Categories
                    .Include(c => c.Products)
                    .ToListAsync(),

                // Get the 5 lowest stock products for the alert table
                LowStockProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Quantity <= p.LowStockThreshold)
                    .OrderBy(p => p.Quantity)
                    .Take(5)
                    .ToListAsync(),

                // Get the 5 most recent orders
                RecentOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}