using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BidsService.Data;
using BidsService.Models;
using BidsService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BidsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuctionServiceClient _auctionServiceClient;

        public BidsController(ApplicationDbContext context, AuctionServiceClient auctionServiceClient)
        {
            _context = context;
            _auctionServiceClient = auctionServiceClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bid>>> GetBids()
        {
            return await _context.Bids.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bid>> GetBid(int id)
        {
            var bid = await _context.Bids.FindAsync(id);

            if (bid == null)
            {
                return NotFound();
            }

            return bid;
        }

        [HttpPost]
        public async Task<ActionResult<Bid>> PostBid(Bid bid)
        {
            // Проверяем, существует ли аукционный элемент
            var auctionItem = await _auctionServiceClient.GetAuctionItemAsync(bid.AuctionItemId);
            if (auctionItem == null)
            {
                return BadRequest("Auction item with the specified ID does not exist.");
            }

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBid), new { id = bid.Id }, bid);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBid(int id, Bid bid)
        {
            if (id != bid.Id)
            {
                return BadRequest();
            }

            _context.Entry(bid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BidExists(id))
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
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BidExists(int id)
        {
            return _context.Bids.Any(e => e.Id == id);
        }
    }
}
