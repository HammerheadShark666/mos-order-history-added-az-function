using MediatR;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microservice.Order.Function.MediatR.DeleteOrderHistory;
using Microsoft.Extensions.Logging;

namespace Microservice.Order.Function.MediatR.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository,  
                                       ILogger<DeleteOrderCommandHandler> logger) : IRequestHandler<DeleteOrderRequest, DeleteOrderResponse>
{
    private IOrderRepository _orderRepository { get; set; } = orderRepository;  
    private ILogger<DeleteOrderCommandHandler> _logger { get; set; } = logger;

    public async Task<DeleteOrderResponse> Handle(DeleteOrderRequest deleteOrderRequest, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(deleteOrderRequest.Id);
        if (order != null)
        {
            await _orderRepository.Delete(order);
        } 
        else
        {
            _logger.LogWarning(String.Format("Order record not found to delete: {0}", deleteOrderRequest.Id.ToString()));
        }

        return new DeleteOrderResponse(); 
    }
} 