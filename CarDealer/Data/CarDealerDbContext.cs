using CarDealer.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.Data
{
    public class CarDealerDbContext : DbContext
    {
        public CarDealerDbContext(DbContextOptions<CarDealerDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().ToTable("Cars");
            modelBuilder.Entity<Inquiry>().ToTable("Inquiries");
        }
    }
    
}
