using Xunit;
using LogiTrack.Models;
using System;

namespace LogiTrack.Tests
{
    public class OrderTests
    {
        [Fact]
        public void AddItem_AddsItemToOrder()
        {
            var order = new Order();
            var item = new InventoryItem { ItemId = 1, Name = "Test", Quantity = 1, Location = "A" };

            order.AddItem(item);

            Assert.Single(order.Items);
            Assert.Equal(item, order.Items[0]);
        }

        [Fact]
        public void RemoveItem_RemovesCorrectItem()
        {
            var order = new Order();
            var item1 = new InventoryItem { ItemId = 1 };
            var item2 = new InventoryItem { ItemId = 2 };
            order.AddItem(item1);
            order.AddItem(item2);

            order.RemoveItem(1);

            Assert.Single(order.Items);
            Assert.Equal(2, order.Items[0].ItemId);
        }

        [Fact]
        public void GetOrderSummary_ReturnsExpectedString()
        {
            var order = new Order
            {
                OrderId = 1001,
                CustomerName = "Samir",
                DatePlaced = new DateTime(2025, 4, 5)
            };
            var item1 = new InventoryItem { ItemId = 1 };
            var item2 = new InventoryItem { ItemId = 2 };
            order.AddItem(item1);
            order.AddItem(item2);
            order.RemoveItem(2); // Now only one item remains

            var summary = order.GetOrderSummary();

            Assert.Contains("Order #1001 for Samir", summary);
            Assert.Contains("Items: 1", summary);
            // Accept any valid short date format for the system culture
            Assert.Contains(order.DatePlaced.ToShortDateString(), summary);
        }
    }
}
