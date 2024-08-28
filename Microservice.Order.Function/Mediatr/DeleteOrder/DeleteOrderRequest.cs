using MediatR;

namespace Microservice.Order.Function.Mediatr.DeleteOrder;

public record DeleteOrderRequest(Guid Id) : IRequest<DeleteOrderResponse>;