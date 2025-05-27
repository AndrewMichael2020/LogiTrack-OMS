using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogiTrack.Models;
using System.Diagnostics;

namespace LogiTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            // Performance improvements:
            // - Use caching (already implemented)
            // - Use AsNoTracking for read-only queries (already implemented)
            // - Consider projecting only needed fields if not all properties are required
            // - Return 304 Not Modified if data hasn't changed (optional, requires ETag or Last-Modified logic)
            // - Use cancellation tokens for better scalability (optional)

            if (!_cache.TryGetValue("inventoryItems", out List<InventoryItem> inventoryItems))
            {
                inventoryItems = await _context.InventoryItems
                    .AsNoTracking()
                    //.Select(i => new InventoryItem { Id = i.Id, Name = i.Name }) // Uncomment and adjust if partial fields suffice
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _cache.Set("inventoryItems", inventoryItems, cacheEntryOptions);
            }

            return inventoryItems;
        }

        // GET: api/inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            // Performance improvements:
            // - Use AsNoTracking for read-only queries (already implemented)
            // - Consider caching individual items if they are frequently accessed (optional)
            var inventoryItem = await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return inventoryItem;
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<InventoryItem>> PostInventoryItem(InventoryItem inventoryItem)
        {
            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();

            // Invalidate cache after data change
            _cache.Remove("inventoryItems");

            return CreatedAtAction("GetInventoryItem", new { id = inventoryItem.Id }, inventoryItem);
        }

        // PUT: api/inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItem(int id, InventoryItem inventoryItem)
        {
            if (id != inventoryItem.Id)
            {
                return BadRequest();
            }

            // Fix: Instead of attaching a new instance, update the existing tracked entity if present, otherwise fetch and update
            var existingEntity = await _context.InventoryItems.FindAsync(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            // Update properties
            _context.Entry(existingEntity).CurrentValues.SetValues(inventoryItem);

            try
            {
                await _context.SaveChangesAsync();
                // Invalidate cache after data change
                _cache.Remove("inventoryItems");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            _context.InventoryItems.Remove(inventoryItem);
            await _context.SaveChangesAsync();

            // Invalidate cache after data change
            _cache.Remove("inventoryItems");

            return NoContent();
        }

        private bool InventoryItemExists(int id)
        {
            // Use Any with AsNoTracking for performance
            return _context.InventoryItems
                .AsNoTracking()
                .Any(e => e.Id == id);
        }
    }
}