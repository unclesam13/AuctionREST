using Microsoft.Extensions.Options;
using PayPal.Api;
using PaymentsService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentsService.Services
{
    public class PayPalService
    {
        private readonly PayPalConfig _config;

        public PayPalService(IOptions<PayPalConfig> config)
        {
            _config = config.Value;
        }

        public async Task<PayPal.Api.Payment> CreatePaymentAsync(decimal amount)
        {
            var apiContext = new APIContext(new OAuthTokenCredential(_config.ClientId, _config.ClientSecret).GetAccessToken())
            {
                Config = new Dictionary<string, string>
                {
                    { "mode", "sandbox" }
                }
            };

            var payment = new PayPal.Api.Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            total = amount.ToString("F2"),
                            currency = "PLN" 
                        },
                        description = "Auction payment"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = "https://localhost:7128/success",
                    cancel_url = "https://localhost:7128/cancel"
                }
            };

            try
            {
                return payment.Create(apiContext);
            }
            catch (PaymentsException ex)
            {
                throw new Exception($"Error connecting to PayPal: {ex.Response}");
            }
        }
    }
}
