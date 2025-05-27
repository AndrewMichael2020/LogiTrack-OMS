using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }

        // EF Core: Foreign key and navigation property for one-to-many
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
        }
    }
}
