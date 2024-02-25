using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<BusinessCategories> Categories { get; set; }
    }
}
