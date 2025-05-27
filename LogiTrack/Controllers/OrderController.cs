using Microsoft.AspNetCore.Mvc;
using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints by default
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public OrderController(LogiTrackContext context)
        {
            _context = context;
        }

        // GET: /api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _context.Orders.Include(o => o.Items).ToListAsync();
            return Ok(orders);
        }

        // GET: /api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = $"Order with ID {id} not found." });

            return Ok(order);
        }

        // POST: /api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
        {
            if (order == null)
                return BadRequest(new { message = "Order payload is required." });

            try
            {
                if (order.Items == null)
                {
                    order.Items = new List<InventoryItem>();
                }

                var attachedItems = new List<InventoryItem>();
                foreach (var item in order.Items)
                {
                    if (item.ItemId != 0)
                    {
                        // Attach only if the item exists in the DB, otherwise add as new
                        var existingItem = await _context.InventoryItems.FindAsync(item.ItemId);
                        if (existingItem != null)
                        {
                            attachedItems.Add(existingItem);
                        }
                        else
                        {
                            // If not found, add as new
                            attachedItems.Add(item);
                        }
                    }
                    else
                    {
                        attachedItems.Add(item);
                    }
                }
                order.Items = attachedItems;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { message = "A database error occurred.", detail = dbEx.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Internal server error.", detail = ex.Message });
            }
        }

        // DELETE: /api/orders/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound(new { message = $"Order with ID {id} not found." });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
