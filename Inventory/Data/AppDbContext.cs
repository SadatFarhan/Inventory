using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }

        public DbSet<Inventorys> Inventories { get; set; }
        public DbSet<Comments> Comment { get; set; }
        public DbSet<Items> Items { get; set; } 
        public DbSet<CustomField> CustomFields { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("User");
            modelBuilder.Entity<Inventorys>().ToTable("Inventory");
        }
    }
    
    
}
