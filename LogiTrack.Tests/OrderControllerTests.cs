using Xunit;
using LogiTrack.Controllers;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogiTrack.Tests
{
    public class OrderControllerTests
    {
        private LogiTrackContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<LogiTrackContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new LogiTrackContext(options); // This requires a matching constructor in LogiTrackContext
        }

        private OrderController GetController(LogiTrackContext context)
        {
            return new OrderController(context);
        }

        [Fact]
        public async Task GetAll_ReturnsAllOrders()
        {
            var context = GetDbContext("GetAll_ReturnsAllOrders");
            context.Orders.Add(new Order { CustomerName = "A", DatePlaced = System.DateTime.Now });
            context.Orders.Add(new Order { CustomerName = "B", DatePlaced = System.DateTime.Now });
            context.SaveChanges();

            var controller = GetController(context);
            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(2, System.Linq.Enumerable.Count(orders));
        }

        [Fact]
        public async Task GetById_ReturnsOrder_WhenExists()
        {
            var context = GetDbContext("GetById_ReturnsOrder_WhenExists");
            var order = new Order { CustomerName = "Samir", DatePlaced = System.DateTime.Now };
            context.Orders.Add(order);
            context.SaveChanges();

            var controller = GetController(context);
            var result = await controller.GetById(order.OrderId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(order.OrderId, returnedOrder.OrderId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var context = GetDbContext("GetById_ReturnsNotFound_WhenMissing");
            var controller = GetController(context);

            var result = await controller.GetById(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task Create_ReturnsCreatedOrder()
        {
            var context = GetDbContext("Create_ReturnsCreatedOrder");
            var controller = GetController(context);

            var order = new Order
            {
                CustomerName = "Test",
                DatePlaced = System.DateTime.Now,
                Items = new List<InventoryItem>
                {
                    new InventoryItem { Name = "Item1", Quantity = 1, Location = "A" }
                }
            };

            var result = await controller.Create(order);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdOrder = Assert.IsType<Order>(created.Value);
            Assert.Equal("Test", createdOrder.CustomerName);
            Assert.Single(createdOrder.Items);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenOrderIsNull()
        {
            var context = GetDbContext("Create_ReturnsBadRequest_WhenOrderIsNull");
            var controller = GetController(context);

            var result = await controller.Create(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("payload", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Delete_RemovesOrder_WhenExists()
        {
            var context = GetDbContext("Delete_RemovesOrder_WhenExists");
            var order = new Order { CustomerName = "Del", DatePlaced = System.DateTime.Now };
            context.Orders.Add(order);
            context.SaveChanges();

            var controller = GetController(context);
            var result = await controller.Delete(order.OrderId);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Orders.FindAsync(order.OrderId));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            var context = GetDbContext("Delete_ReturnsNotFound_WhenMissing");
            var controller = GetController(context);

            var result = await controller.Delete(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task Create_OrderWithExistingItem_AttachesItem()
        {
            var context = GetDbContext("Create_OrderWithExistingItem_AttachesItem");
            var existingItem = new InventoryItem { Name = "Existing", Quantity = 2, Location = "B" };
            context.InventoryItems.Add(existingItem);
            context.SaveChanges();

            var controller = GetController(context);
            var order = new Order
            {
                CustomerName = "Attach",
                DatePlaced = System.DateTime.Now,
                Items = new List<InventoryItem>
                {
                    new InventoryItem { ItemId = existingItem.ItemId }
                }
            };

            var result = await controller.Create(order);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdOrder = Assert.IsType<Order>(created.Value);
            Assert.Single(createdOrder.Items);
            Assert.Equal(existingItem.ItemId, createdOrder.Items[0].ItemId);
        }
    }
}
