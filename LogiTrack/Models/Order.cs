using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public DateTime DatePlaced { get; set; }

        // EF Core: One-to-many relationship
        public List<InventoryItem> Items { get; set; } = new();

        public void AddItem(InventoryItem item)
        {
            // Performance: Avoid duplicate adds
            if (!Items.Any(i => i.ItemId == item.ItemId))
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int itemId)
        {
            var item = Items.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        // Efficient summary using string interpolation
        public string GetOrderSummary()
        {
            return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced:d}";
        }
    }
}
