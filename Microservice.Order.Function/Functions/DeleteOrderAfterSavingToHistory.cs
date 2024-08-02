using Azure.Messaging.ServiceBus;
using MediatR;
using Microservice.Order.Function.Helpers;
using Microservice.Order.Function.MediatR.DeleteOrderHistory;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Microservice.Order.Function.Functions
{
    public class DeleteOrderAfterSavingToHistory(ILogger<DeleteOrderAfterSavingToHistory> logger, IMediator mediator)
    {
        private record Order(Guid OrderId);

        private ILogger<DeleteOrderAfterSavingToHistory> _logger { get; set; } = logger;
        private IMediator _mediator { get; set; } = mediator;

        [Function(nameof(DeleteOrderAfterSavingToHistory))]
        public async Task Run([ServiceBusTrigger(Constants.AzureServiceBusQueueOrderHistoryAdded,
                                                 Connection = Constants.AzureServiceBusConnection)]
                                                 ServiceBusReceivedMessage message)
                                                 //ServiceBusMessageActions messageActions)
        { 
            var orderId = GetOrderId(message.Body.ToArray());

            _logger.LogInformation(string.Format("Order History Added - Delete Order - {0}", orderId.ToString()));

            try
            {
                throw new Exception("exception");
               // await _mediator.Send(new DeleteOrderRequest(orderId));
               // await messageActions.CompleteMessageAsync(message);

                //_logger.LogInformation("Order deleted: " + orderId.ToString());

                //return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("Internal Error: Id: {0} - {1}", orderId.ToString(), ex.Message));
                //await messageActions.DeadLetterMessageAsync(message, null, Constants.FailureReasonInternal, ex.Message);
            }
        }

        private Guid GetOrderId(byte[] message)
        {
            var order = JsonHelper.GetRequest<Order>(message);
            if (order == null)
                throw new ArgumentNullException("Order not in message.");

            return order.OrderId;

        }
    }
}
