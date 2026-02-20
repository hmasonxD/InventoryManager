using InventoryManager.Models;

namespace InventoryManager.Tests
{
    public class OrderTests
    {
        [Fact]
        public void OrderItem_Subtotal_CalculatesCorrectly()
        {
            var item = new OrderItem
            {
                Quantity = 3,
                UnitPrice = 25.50m
            };

            Assert.Equal(76.50m, item.Subtotal);
        }

        [Fact]
        public void Order_TotalAmount_SumsAllItems()
        {
            var order = new Order
            {
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Quantity = 2, UnitPrice = 10.00m },
                    new OrderItem { Quantity = 1, UnitPrice = 25.00m },
                    new OrderItem { Quantity = 3, UnitPrice = 5.50m }
                }
            };

            // (2 * 10) + (1 * 25) + (3 * 5.50) = 20 + 25 + 16.50 = 61.50
            Assert.Equal(61.50m, order.TotalAmount);
        }

        [Fact]
        public void Order_TotalAmount_ReturnsZeroWithNoItems()
        {
            var order = new Order();

            Assert.Equal(0m, order.TotalAmount);
        }

        [Fact]
        public void Order_DefaultStatus_IsPending()
        {
            var order = new Order();

            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public void Order_DefaultDate_IsSetToNow()
        {
            var before = DateTime.Now.AddSeconds(-1);
            var order = new Order();
            var after = DateTime.Now.AddSeconds(1);

            Assert.InRange(order.OrderDate, before, after);
        }
    }
}