using Microsoft.EntityFrameworkCore;
using BidsService.Models;

namespace BidsService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bid>(entity =>
            {
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)"); // Указываем тип данных
            });
        }
    }
}
