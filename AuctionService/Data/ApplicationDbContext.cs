using Microsoft.EntityFrameworkCore;
using AuctionService.Models;

namespace AuctionService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuctionItem> AuctionItems { get; set; }
    }
}
