using Azure.Messaging.ServiceBus;
using MediatR;
using Microservice.Order.Function.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Microservice.Order.Function.Test.Unit.Functions;

public class DeleteOrderFromOrderHistoryAddedAzureFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<DeleteOrderAfterSavingToHistory>> _mockLogger;
    private readonly DeleteOrderAfterSavingToHistory _deleteOrderAfterSavingToHistory;
    private record Order(string OrderId);
    public DeleteOrderFromOrderHistoryAddedAzureFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<DeleteOrderAfterSavingToHistory>>();
        _deleteOrderAfterSavingToHistory = new DeleteOrderAfterSavingToHistory(_mockLogger.Object, _mockMediator.Object);
    }

    [Test, Order(1)]
    public async Task Azure_function_trigger_service_bus_receive_return_succeed()
    {
        var deleteOrder = new Order("724cbd34-3dff-4e2a-a413-48825f1ab3b9");

        var mockMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonConvert.SerializeObject(deleteOrder)), correlationId: Guid.NewGuid().ToString());

        var mockServiceBusMessageActions = new Mock<ServiceBusMessageActions>();
        mockServiceBusMessageActions.Setup(x => x.CompleteMessageAsync(mockMessage, CancellationToken.None)).Returns(Task.FromResult(true));

        await _deleteOrderAfterSavingToHistory.Run(mockMessage, mockServiceBusMessageActions.Object);
    }
}