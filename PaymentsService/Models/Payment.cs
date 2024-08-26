namespace PaymentsService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int AuctionItemId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; 
        public DateTime PaymentDate { get; set; }
    }
}
