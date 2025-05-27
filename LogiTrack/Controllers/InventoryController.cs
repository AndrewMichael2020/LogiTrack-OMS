using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LogiTrack.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints by default
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly IMemoryCache _cache;

        public InventoryController(LogiTrackContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
        {
            // Caching strategy: Use IMemoryCache with longer expiration and rehydration logic
            if (!_cache.TryGetValue("inventoryItems", out List<InventoryItem> inventoryItems))
            {
                inventoryItems = await _context.InventoryItems
                    .AsNoTracking()
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(System.TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(System.TimeSpan.FromHours(1)); // Absolute expiration for safety
                _cache.Set("inventoryItems", inventoryItems, cacheEntryOptions);
            }

            return inventoryItems;
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<InventoryItem>> PostInventoryItem(InventoryItem inventoryItem)
        {
            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();

            // Rehydrate cache after data change
            var inventoryItems = await _context.InventoryItems
                .AsNoTracking()
                .ToListAsync();
            _cache.Set("inventoryItems", inventoryItems, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(System.TimeSpan.FromMinutes(10)));

            return CreatedAtAction(nameof(GetInventoryItems), new { id = inventoryItem.Id }, inventoryItem);
        }

        // PUT: api/inventory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItem(int id, InventoryItem inventoryItem)
        {
            if (id != inventoryItem.Id)
                return BadRequest();

            var existingEntity = await _context.InventoryItems.FindAsync(id);
            if (existingEntity == null)
                return NotFound();

            _context.Entry(existingEntity).CurrentValues.SetValues(inventoryItem);

            try
            {
                await _context.SaveChangesAsync();

                // Rehydrate cache after data change
                var inventoryItems = await _context.InventoryItems
                    .AsNoTracking()
                    .ToListAsync();
                _cache.Set("inventoryItems", inventoryItems, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(System.TimeSpan.FromMinutes(10)));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.InventoryItems.Any(e => e.ItemId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/inventory/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            // Rehydrate cache after data change
            var inventoryItems = await _context.InventoryItems
                .AsNoTracking()
                .ToListAsync();
            _cache.Set("inventoryItems", inventoryItems, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(System.TimeSpan.FromMinutes(10)));

            return NoContent();
        }
    }
}
