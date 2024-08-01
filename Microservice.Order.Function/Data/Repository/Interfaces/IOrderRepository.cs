namespace Microservice.Order.Function.Data.Repository.Interfaces;

public interface IOrderRepository
{ 
    Task Delete(Domain.Order order);
    Task<Domain.Order> GetByIdAsync(Guid id);  
}