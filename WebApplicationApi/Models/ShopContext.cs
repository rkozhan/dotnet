using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationApi.Models
{
    public class ShopContext : DbContext
    {
        // Constructor to configure the DbContext with options
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        // Configuring the entity relationships and other model settings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a one-to-many relationship between Category and Product
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)  // A Category has many Products
                .WithOne(a => a.Category)  // Each Product belongs to one Category
                .HasForeignKey(a => a.CategoryId); // Foreign key in Product

            modelBuilder.Seed(); // Initialize the database with seed data
        }

        // DbSet properties for querying and saving instances of the entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
