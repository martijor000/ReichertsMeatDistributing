using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<WeeklyDeal> WeeklyDeals { get; set; }
        public DbSet<ProductItem> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure WeeklyDeal
            modelBuilder.Entity<WeeklyDeal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            // Configure ProductItem - work with existing table structure
            modelBuilder.Entity<ProductItem>(entity =>
            {
                entity.HasKey(e => e.ItemID);
                entity.Property(e => e.ItemID).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ItemDescription).IsRequired().HasMaxLength(500);
                entity.Property(e => e.StockingUM).IsRequired().HasMaxLength(50);
                
                // New fields - these will be added via migration
                entity.Property(e => e.Category).HasDefaultValue(BusinessCategory.All);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.ModifiedDate);
            });


        }
    }
} 