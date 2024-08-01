using Microsoft.EntityFrameworkCore;

namespace Microservice.Order.Function.Data.Contexts;

public class OrderDbContext : DbContext
{ 
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
 
    public DbSet<Domain.Order> Orders { get; set; }
    public DbSet<Domain.OrderItem> OrderItems { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        base.OnModelCreating(modelBuilder);  

        modelBuilder.Entity<Domain.Order>().HasMany(e => e.OrderItems);
        modelBuilder.Entity<Domain.OrderItem>().HasKey(e => new { e.OrderId, e.ProductId }); 
    }
}

//add-migration
//update-database

//azurite --silent --location c:\azurite --debug c:\azurite\debug.log