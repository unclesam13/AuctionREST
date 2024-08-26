using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;
using PaymentsService.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PayPalService _payPalService;
        private readonly HttpClient _httpClient;

        public PaymentsController(ApplicationDbContext context, PayPalService payPalService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _payPalService = payPalService;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            if (!await UserExists(payment.UserId))
            {
                return BadRequest("User not found");
            }

            if (!await AuctionItemExists(payment.AuctionItemId))
            {
                return BadRequest("Auction item not found");
            }

            // Проверка баланса пользователя (например, 5000 PLN)
            var userBalance = 5000m;
            if (payment.Amount > userBalance)
            {
                return BadRequest("Insufficient funds");
            }

            var payPalResponse = await _payPalService.CreatePaymentAsync(payment.Amount);

            payment.Status = payPalResponse.state;
            payment.PaymentDate = DateTime.UtcNow;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }

        private async Task<bool> UserExists(int userId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:5001/api/user/{userId}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> AuctionItemExists(int auctionItemId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:5003/api/auctionitems/{auctionItemId}");
            return response.IsSuccessStatusCode;
        }
    }
}
