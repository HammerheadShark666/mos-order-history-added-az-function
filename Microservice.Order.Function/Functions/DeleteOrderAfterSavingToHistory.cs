using Azure.Messaging.ServiceBus;
using MediatR;
using Microservice.Order.Function.Helpers;
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
                                                 Connection = Constants.AzureServiceBusConnectionManagedIdentity, AutoCompleteMessages = false)]
                                                 ServiceBusReceivedMessage message,
                                                 ServiceBusMessageActions messageActions)
        {
            var deleteOrderRequest = GetDeleteOrderRequest(message.Body.ToArray());

            logger.LogInformation("Order History Added - Delete Order - {orderId}", deleteOrderRequest.Id);

            try
            {
                await mediator.Send(deleteOrderRequest);
                await messageActions.CompleteMessageAsync(message);

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Internal Error: Id: {orderId} - {ex.Message}", deleteOrderRequest.Id, ex.Message);
                await messageActions.DeadLetterMessageAsync(message, null, Constants.FailureReasonInternal, ex.StackTrace);
            }
        }

        private static DeleteOrderRequest GetDeleteOrderRequest(byte[] message)
        {
            var order = JsonHelper.GetRequest<Order>(message) ?? throw new ArgumentNullException(nameof(message), "Order id not in message.");
            return new DeleteOrderRequest(new Guid(order.OrderId));
        }
    }
}