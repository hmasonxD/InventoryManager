using InventoryManager.Models;

namespace InventoryManager.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Product_IsLowStock_ReturnsTrueWhenAtThreshold()
        {
            // Arrange — set up the test data
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 10,
                LowStockThreshold = 10
            };

            // Act & Assert
            Assert.True(product.IsLowStock);
        }

        [Fact]
        public void Product_IsLowStock_ReturnsTrueWhenBelowThreshold()
        {
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 3,
                LowStockThreshold = 10
            };

            Assert.True(product.IsLowStock);
        }

        [Fact]
        public void Product_IsLowStock_ReturnsFalseWhenAboveThreshold()
        {
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 50,
                LowStockThreshold = 10
            };

            Assert.False(product.IsLowStock);
        }

        [Fact]
        public void Product_IsLowStock_ReturnsTrueWhenQuantityIsZero()
        {
            var product = new Product
            {
                Name = "Test Product",
                Quantity = 0,
                LowStockThreshold = 10
            };

            Assert.True(product.IsLowStock);
        }

        [Theory]
        [InlineData(5, 10, true)]     // below threshold
        [InlineData(10, 10, true)]    // at threshold
        [InlineData(11, 10, false)]   // above threshold
        [InlineData(0, 0, true)]      // both zero
        [InlineData(100, 5, false)]   // well above
        public void Product_IsLowStock_VariousScenarios(int quantity, int threshold, bool expected)
        {
            var product = new Product
            {
                Name = "Test Product",
                Quantity = quantity,
                LowStockThreshold = threshold
            };

            Assert.Equal(expected, product.IsLowStock);
        }
    }
}