using Azure.Messaging.ServiceBus;
using MediatR;
using Microservice.Order.Function.Helpers;
using Microservice.Order.Function.Helpers.Exceptions;
using Microservice.Order.Function.Mediatr.DeleteOrder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Microservice.Order.Function.Functions
{
    public class DeleteOrderAfterSavingToHistory(ILogger<DeleteOrderAfterSavingToHistory> logger, IMediator mediator)
    {
        private record Order(string OrderId);

        [Function(nameof(DeleteOrderAfterSavingToHistory))]
        public async Task Run([ServiceBusTrigger("%" + Constants.AzureServiceBusQueueOrderHistoryAdded + "%",
                                                 Connection = Constants.AzureServiceBusConnection, AutoCompleteMessages = false)]
                                                 ServiceBusReceivedMessage message,
                                                 ServiceBusMessageActions messageActions)
        {
            var orderId = GetOrderId(message.Body.ToArray());

            logger.LogInformation("Order History Added - Delete Order - {orderId}", orderId);

            try
            {
                await mediator.Send(new DeleteOrderRequest(orderId));
                await messageActions.CompleteMessageAsync(message);

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Internal Error: Id: {orderId} - {ex.Message}", orderId, ex.Message);
                await messageActions.DeadLetterMessageAsync(message, null, Constants.FailureReasonInternal, ex.StackTrace);
            }
        }

        private static Guid GetOrderId(byte[] message)
        {
            Order? order = JsonHelper.GetRequest<Order>([.. message]) ?? throw new JsonDeserializeException("Error deserializing Order.");
            return new Guid(order.OrderId);
        }
    }
}