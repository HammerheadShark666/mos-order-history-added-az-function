using Microservice.Order.Function.Data.Contexts;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Order.Function.Data.Repository;

public class OrderRepository(IDbContextFactory<OrderDbContext> dbContextFactory) : IOrderRepository
{    
    public IDbContextFactory<OrderDbContext> _dbContextFactory { get; set; } = dbContextFactory;
      
    public async Task Delete(Domain.Order order)
    { 
        using var db = _dbContextFactory.CreateDbContext(); 
         
        db.Orders.Remove(order);
        await db.SaveChangesAsync(); 
    }

    public async Task<Domain.Order> GetByIdAsync(Guid id)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Orders
                        .Where(o => o.Id.Equals(id))
                        .Include(e => e.OrderItems)
                        .Include("OrderItems.ProductType")
                        .Include(e => e.OrderStatus)
                        .SingleOrDefaultAsync();
    }
}