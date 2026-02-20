using Microsoft.EntityFrameworkCore;
using InventoryManager.Controllers;
using InventoryManager.Data;
using InventoryManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Tests
{
    public class DashboardControllerTests
    {
        // Helper method — creates a fresh in-memory database for each test
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCorrectProductCount()
        {
            // Arrange
            var context = GetDbContext();

            // Seed test data
            var category = new Category { Name = "Electronics" };
            var supplier = new Supplier { CompanyName = "TestCorp" };
            context.Categories.Add(category);
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            context.Products.AddRange(
                new Product { Name = "Laptop", Price = 999, Quantity = 50, CategoryId = category.Id, SupplierId = supplier.Id },
                new Product { Name = "Mouse", Price = 25, Quantity = 200, CategoryId = category.Id, SupplierId = supplier.Id }
            );
            await context.SaveChangesAsync();

            var controller = new DashboardController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);
            Assert.Equal(2, model.TotalProducts);
        }

        [Fact]
        public async Task Index_CalculatesInventoryValueCorrectly()
        {
            var context = GetDbContext();

            var category = new Category { Name = "Test" };
            var supplier = new Supplier { CompanyName = "Test" };
            context.Categories.Add(category);
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            context.Products.AddRange(
                new Product { Name = "A", Price = 10, Quantity = 5, CategoryId = category.Id, SupplierId = supplier.Id },
                new Product { Name = "B", Price = 20, Quantity = 3, CategoryId = category.Id, SupplierId = supplier.Id }
            );
            await context.SaveChangesAsync();

            var controller = new DashboardController(context);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);

            // (10 * 5) + (20 * 3) = 50 + 60 = 110
            Assert.Equal(110m, model.TotalInventoryValue);
        }

        [Fact]
        public async Task Index_DetectsLowStockProducts()
        {
            var context = GetDbContext();

            var category = new Category { Name = "Test" };
            var supplier = new Supplier { CompanyName = "Test" };
            context.Categories.Add(category);
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            context.Products.AddRange(
                new Product { Name = "Low", Price = 10, Quantity = 2, LowStockThreshold = 10, CategoryId = category.Id, SupplierId = supplier.Id },
                new Product { Name = "OK", Price = 20, Quantity = 100, LowStockThreshold = 10, CategoryId = category.Id, SupplierId = supplier.Id }
            );
            await context.SaveChangesAsync();

            var controller = new DashboardController(context);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);
            Assert.Equal(1, model.LowStockCount);
        }

        [Fact]
        public async Task Index_ReturnsEmptyDashboardWithNoData()
        {
            var context = GetDbContext();
            var controller = new DashboardController(context);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);
            Assert.Equal(0, model.TotalProducts);
            Assert.Equal(0, model.TotalOrders);
            Assert.Equal(0m, model.TotalInventoryValue);
        }
    }
}