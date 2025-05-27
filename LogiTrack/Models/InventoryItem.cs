using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemId { get; set; }
        // Add property alias for test compatibility
        [NotMapped]
        public int Id
        {
            get => ItemId;
            set => ItemId = value;
        }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Location { get; set; } = string.Empty;

        // EF Core: Foreign key and navigation property for one-to-many
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
        }
    }
}
