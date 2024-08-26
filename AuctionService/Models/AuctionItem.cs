namespace AuctionService.Models
{
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
