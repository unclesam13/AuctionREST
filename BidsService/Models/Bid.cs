namespace BidsService.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public decimal Amount { get; set; } // Сумма ставки
        public int UserId { get; set; } // Идентификатор пользователя, сделавшего ставку
        public int AuctionItemId { get; set; } // Идентификатор лота аукциона
    }
}
