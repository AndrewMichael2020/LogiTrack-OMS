using LogiTrack.Models;

// Seed the database with a test inventory item if none exist
using (var context = new LogiTrackContext())
{
    // Add test inventory item if none exist
    if (!context.InventoryItems.Any())
    {
        var item = new InventoryItem
        {
            Name = "Pallet Jack",
            Quantity = 12,
            Location = "Warehouse A"
        };
        context.InventoryItems.Add(item);
        context.SaveChanges();
    }

    // Print all inventory items
    foreach (var item in context.InventoryItems)
    {
        item.DisplayInfo();
    }
}

// Test block for InventoryItem
var item = new InventoryItem
{
    ItemId = 1,
    Name = "Pallet Jack",
    Quantity = 12,
    Location = "Warehouse A"
};
item.DisplayInfo();

// Test block for Order
var order = new Order
{
    OrderId = 1001,
    CustomerName = "Samir",
    DatePlaced = new DateTime(2025, 4, 5)
};
order.AddItem(item);
order.AddItem(new InventoryItem
{
    ItemId = 2,
    Name = "Hand Truck",
    Quantity = 5,
    Location = "Warehouse B"
});
order.RemoveItem(2);
Console.WriteLine(order.GetOrderSummary());