using Microservice.Customer.Address.Function.Helpers.Exceptions;
using Microservice.Order.Function.Data.Context;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Order.Function.Data.Repository;

public class OrderRepository(IDbContextFactory<OrderDbContext> dbContextFactory) : IOrderRepository
{
    public async Task Delete(Domain.Order order)
    {
        using var db = dbContextFactory.CreateDbContext();

        db.Orders.Remove(order);
        await db.SaveChangesAsync();
    }

    public async Task<Domain.Order> GetByIdAsync(Guid id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var order = await db.Orders
                        .Where(o => o.Id.Equals(id))
                        .Include(e => e.OrderItems)
                        .Include("OrderItems.ProductType")
                        .Include(e => e.OrderStatus)
                        .SingleOrDefaultAsync() ?? throw new NotFoundException("Order not found.");
        return order;
    }
}