using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogiTrack.Controllers;
using LogiTrack.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class InventoryControllerTests
{
    private InventoryController GetControllerWithData(List<InventoryItem> items, out LogiTrackContext context, out IMemoryCache cache)
    {
        var options = new DbContextOptionsBuilder<LogiTrackContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
            .Options;
        context = new LogiTrackContext(options);
        context.InventoryItems.AddRange(items);
        context.SaveChanges();
        cache = new MemoryCache(new MemoryCacheOptions());
        return new InventoryController(context, cache);
    }

    [Fact]
    public async Task GetInventoryItems_CachesResult()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new InventoryItem { Id = 1, Name = "Item1" }
        };
        var controller = GetControllerWithData(items, out var context, out var cache);

        // Act - First call, should fetch from DB and cache
        var result1 = await controller.GetInventoryItems();
        Assert.Single(result1.Value);

        // Add another item directly to DB (simulate external change)
        context.InventoryItems.Add(new InventoryItem { Id = 2, Name = "Item2" });
        context.SaveChanges();

        // Act - Second call, should return cached result (still 1 item)
        var result2 = await controller.GetInventoryItems();
        Assert.Single(result2.Value);

        // Invalidate cache and check updated result
        cache.Remove("inventoryItems");
        var result3 = await controller.GetInventoryItems();
        Assert.Equal(2, result3.Value.Count());
    }

    [Fact]
    public async Task PostInventoryItem_InvalidatesCache()
    {
        var items = new List<InventoryItem>
        {
            new InventoryItem { Id = 1, Name = "Item1" }
        };
        var controller = GetControllerWithData(items, out var context, out var cache);

        // Prime cache
        var _ = await controller.GetInventoryItems();

        // Add new item via POST
        var newItem = new InventoryItem { Id = 2, Name = "Item2" };
        await controller.PostInventoryItem(newItem);

        // Now, cache should be invalidated and new item should appear
        var result = await controller.GetInventoryItems();
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task PutInventoryItem_InvalidatesCache()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new InventoryItem { Id = 1, Name = "Item1" }
        };
        var controller = GetControllerWithData(items, out var context, out var cache);

        // Prime cache
        var _ = await controller.GetInventoryItems();

        // Act - Update item via PUT
        var updatedItem = new InventoryItem { Id = 1, Name = "UpdatedItem" };
        await controller.PutInventoryItem(1, updatedItem);

        // Assert - Cache should be invalidated and updated item should appear
        var result = await controller.GetInventoryItems();
        Assert.Equal("UpdatedItem", result.Value.First().Name);
    }

    [Fact]
    public async Task DeleteInventoryItem_InvalidatesCache()
    {
        var items = new List<InventoryItem>
        {
            new InventoryItem { Id = 1, Name = "Item1" }
        };
        var controller = GetControllerWithData(items, out var context, out var cache);

        // Prime cache
        var _ = await controller.GetInventoryItems();

        // Delete item via DELETE
        await controller.DeleteInventoryItem(1);

        // Cache should be invalidated, and no items should be returned
        var result = await controller.GetInventoryItems();
        Assert.Empty(result.Value);
    }
}