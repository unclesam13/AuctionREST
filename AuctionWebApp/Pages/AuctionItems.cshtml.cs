using AuctionService.Models; 
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace AuctionWebApp.Pages
{
    public class AuctionItemsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AuctionItemsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            AuctionItems = new List<AuctionItem>(); 
            NewAuctionItem = new AuctionItem();
        }

        public List<AuctionItem> AuctionItems { get; set; }

        [BindProperty]
        public AuctionItem NewAuctionItem { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://localhost:5003/api/auctionitems");
                AuctionItems = JsonConvert.DeserializeObject<List<AuctionItem>>(response);
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to API: " + httpEx.Message);
            }
            catch (NotSupportedException notSupEx)
            {
                ModelState.AddModelError(string.Empty, "Content format not supported: " + notSupEx.Message);
            }
            catch (JsonException jsonEx)
            {
                ModelState.AddModelError(string.Empty, "JSON deserialization error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error has occurred: " + ex.Message);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/api/auctionitems", NewAuctionItem);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error adding new auction item");
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to API: " + httpEx.Message);
            }
            catch (NotSupportedException notSupEx)
            {
                ModelState.AddModelError(string.Empty, "Content format not supported: " + notSupEx.Message);
            }
            catch (JsonException jsonEx)
            {
                ModelState.AddModelError(string.Empty, "JSON deserialization error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error has occurred: " + ex.Message);
            }

            return Page();
        }
    }
}
