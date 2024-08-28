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

        private ILogger<DeleteOrderAfterSavingToHistory> _logger { get; set; } = logger;
        private IMediator _mediator { get; set; } = mediator;

        [Function(nameof(DeleteOrderAfterSavingToHistory))]
        public async Task Run([ServiceBusTrigger("%" + Constants.AzureServiceBusQueueOrderHistoryAdded + "%",
                                                 Connection = Constants.AzureServiceBusConnection, AutoCompleteMessages = false)]
                                                 ServiceBusReceivedMessage message,
                                                 ServiceBusMessageActions messageActions)
        {
            var orderId = GetOrderId(message.Body.ToArray());

            _logger.LogInformation($"Order History Added - Delete Order - {orderId}");

            try
            {
                await _mediator.Send(new DeleteOrderRequest(orderId));
                await messageActions.CompleteMessageAsync(message);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal Error: Id: {orderId} - {ex.Message}");
                await messageActions.DeadLetterMessageAsync(message, null, Constants.FailureReasonInternal, ex.StackTrace);
            }
        }

        private static Guid GetOrderId(byte[] message)
        {
            var order = JsonHelper.GetRequest<Order>(message) ?? throw new ArgumentNullException("Order not in message.");
            return new Guid(order.OrderId);

        }
    }
}