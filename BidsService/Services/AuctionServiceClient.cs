using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BidsService.Services
{
    public class AuctionServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuctionServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuctionItem> GetAuctionItemAsync(int auctionItemId)
        {
            var response = await _httpClient.GetAsync($"/api/AuctionItems/{auctionItemId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuctionItem>();
            }
            else
            {
                return null;
            }
        }
    }

    public class AuctionItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
    }
}
