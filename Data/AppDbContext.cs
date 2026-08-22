using Microsoft.EntityFrameworkCore;
using OrderSystem.Models;

namespace OrderSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=ordersystem.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(p => p.Id);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = "P01", Name = "Ao thun", Price = 150000, Category = "clothing", Stock = 20 },
            new Product { Id = "P02", Name = "Quan jean", Price = 450000, Category = "clothing", Stock = 10 },
            new Product { Id = "P03", Name = "Tai nghe", Price = 890000, Category = "electronics", Stock = 5 },
            new Product { Id = "P04", Name = "Sac du phong", Price = 350000, Category = "electronics", Stock = 8 }
        );
    }
}
