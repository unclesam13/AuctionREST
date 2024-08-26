using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionService.Data;
using AuctionService.Models;
using AuctionService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserServiceClient _userServiceClient;

        public AuctionItemsController(ApplicationDbContext context, UserServiceClient userServiceClient)
        {
            _context = context;
            _userServiceClient = userServiceClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionItem>>> GetAuctionItems()
        {
            return await _context.AuctionItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionItem>> GetAuctionItem(int id)
        {
            var auctionItem = await _context.AuctionItems.FindAsync(id);

            if (auctionItem == null)
            {
                return NotFound();
            }

            return auctionItem;
        }

        [HttpPost]
        public async Task<ActionResult<AuctionItem>> PostAuctionItem(AuctionItem auctionItem)
        {
            // Проверяем, существует ли пользователь
            var user = await _userServiceClient.GetUserAsync(auctionItem.UserId);
            if (user == null)
            {
                return BadRequest("User with the specified ID does not exist.");
            }

            _context.AuctionItems.Add(auctionItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuctionItem), new { id = auctionItem.Id }, auctionItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuctionItem(int id, AuctionItem auctionItem)
        {
            if (id != auctionItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(auctionItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionItemExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionItem(int id)
        {
            var auctionItem = await _context.AuctionItems.FindAsync(id);
            if (auctionItem == null)
            {
                return NotFound();
            }

            _context.AuctionItems.Remove(auctionItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuctionItemExists(int id)
        {
            return _context.AuctionItems.Any(e => e.Id == id);
        }
    }
}
