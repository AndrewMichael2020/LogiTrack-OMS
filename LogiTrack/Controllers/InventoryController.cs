using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogiTrack.Models;
using System.Collections.Generic;
using System.Linq;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints by default
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public InventoryController(LogiTrackContext context)
        {
            _context = context;
        }

        // GET: /api/inventory
        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> GetAll()
        {
            return Ok(_context.InventoryItems.ToList());
        }

        // POST: /api/inventory
        [HttpPost]
        public ActionResult<InventoryItem> Create(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = item.ItemId }, item);
        }

        // DELETE: /api/inventory/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult Delete(int id)
        {
            var item = _context.InventoryItems.Find(id);
            if (item == null)
                return NotFound();

            _context.InventoryItems.Remove(item);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
