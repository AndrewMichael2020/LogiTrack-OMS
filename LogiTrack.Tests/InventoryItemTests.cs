using Xunit;
using LogiTrack.Models;
using System;
using System.IO;

namespace LogiTrack.Tests
{
    public class InventoryItemTests
    {
        [Fact]
        public void DisplayInfo_PrintsCorrectFormat()
        {
            var item = new InventoryItem
            {
                Name = "Pallet Jack",
                Quantity = 12,
                Location = "Warehouse A"
            };
            using var sw = new StringWriter();
            Console.SetOut(sw);

            item.DisplayInfo();

            var expected = "Item: Pallet Jack | Quantity: 12 | Location: Warehouse A" + Environment.NewLine;
            Assert.Equal(expected, sw.ToString());
        }
    }
}
