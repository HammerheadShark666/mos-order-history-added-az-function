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
        private ILogger<DeleteOrderAfterSavingToHistory> _logger { get; set; } = logger;
        private IMediator _mediator { get; set; } = mediator;

        [Function(nameof(DeleteOrderAfterSavingToHistory))]
        public async Task Run([ServiceBusTrigger(Constants.AzureServiceBusQueueOrderHistoryAdded,
                                                 Connection = Constants.AzureServiceBusConnection)]
                                                 ServiceBusReceivedMessage message,
                                                 ServiceBusMessageActions messageActions)
        {
            var orderId = JsonHelper.GetRequest<Guid>(message.Body.ToArray());

            try
            {
                _logger.LogInformation(string.Format("Order History Added - Delete Order - {0}", orderId.ToString()));

                await _mediator.Send(new DeleteOrderRequest(orderId));
                await messageActions.CompleteMessageAsync(message);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("Internal Error: Id: {0}", orderId.ToString()));
                await messageActions.DeadLetterMessageAsync(message, null, Constants.FailureReasonInternal, ex.Message);
            }
        }
    }
}
