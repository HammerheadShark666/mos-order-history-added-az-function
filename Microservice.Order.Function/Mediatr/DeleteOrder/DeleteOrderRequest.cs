using MediatR;

namespace Microservice.Order.Function.MediatR.DeleteOrderHistory;

public record DeleteOrderRequest(Guid Id) : IRequest<DeleteOrderResponse>;