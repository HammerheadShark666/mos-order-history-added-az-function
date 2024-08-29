using MediatR;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microservice.Order.Function.Mediatr.DeleteOrder;
using Microsoft.Extensions.Logging;

namespace Microservice.Order.Function.MediatR.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository,
                                       ILogger<DeleteOrderCommandHandler> logger) : IRequestHandler<DeleteOrderRequest, DeleteOrderResponse>
{
    public async Task<DeleteOrderResponse> Handle(DeleteOrderRequest deleteOrderRequest, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(deleteOrderRequest.Id);
        if (order != null)
        {
            await orderRepository.Delete(order);
        }
        else
        {
            logger.LogWarning("Order record not found to delete: {deleteOrderRequest.Id}.", deleteOrderRequest.Id);
        }

        return new DeleteOrderResponse();
    }
}