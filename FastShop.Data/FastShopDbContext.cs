using Microsoft.EntityFrameworkCore;
using FastShop.Data.Entities;

namespace FastShop.Data;

public class FastShopDbContext : DbContext
{
    public FastShopDbContext(DbContextOptions<FastShopDbContext> options) : base(options)
    { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<SizeType> SizeTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SizeType>().HasData(
            new SizeType { Id = 1, Name = "Paper" },
            new SizeType { Id = 2, Name = "Clothing" }
        );

        modelBuilder.Entity<Size>().HasData(
            new Size { Id = 1, Name = "A4", SizeTypeId = 1 },
            new Size { Id = 2, Name = "A3", SizeTypeId = 1 },
            new Size { Id = 3, Name = "Small", SizeTypeId = 2 },
            new Size { Id = 3, Name = "Medium", SizeTypeId = 2 },
            new Size { Id = 3, Name = "Large", SizeTypeId = 2 },
            new Size { Id = 3, Name = "X-Large", SizeTypeId = 2 }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Clothing" },
            new Category { Id = 3, Name = "Groceries" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m, CategoryId = 1 },
            new Product { Id = 2, Name = "T-Shirt", Price = 20.00m, CategoryId = 2 },
            new Product { Id = 3, Name = "Apples", Price = 5.00m, CategoryId = 3 },
            new Product { Id = 4, Name = "Laptop", Price = 1200.00m, CategoryId = 1 },
            new Product { Id = 5, Name = "T-Shirt", Price = 20.00m, CategoryId = 2 },
            new Product { Id = 6, Name = "Apples", Price = 5.00m, CategoryId = 3 },
            new Product { Id = 7, Name = "Laptop", Price = 1200.00m, CategoryId = 1 },
            new Product { Id = 8, Name = "T-Shirt", Price = 20.00m, CategoryId = 2 },
            new Product { Id = 9, Name = "Apples", Price = 5.00m, CategoryId = 3 }
        );
    }
}