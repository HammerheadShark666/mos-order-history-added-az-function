using MediatR;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microservice.Order.Function.Mediatr.DeleteOrder;
using Microservice.Order.Function.MediatR.DeleteOrder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Microservice.Order.Function.Test.Unit.Mediatr;

[TestFixture]
public class DeleteOrderFromOrderHistoryAddedMediatrTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock = new();
    private readonly Mock<ILogger<DeleteOrderCommandHandler>> loggerMock = new();
    private readonly ServiceCollection services = new();
    private ServiceProvider serviceProvider;
    private IMediator mediator;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DeleteOrderCommandHandler).Assembly));
        services.AddScoped(sp => orderRepositoryMock.Object);
        services.AddScoped(sp => loggerMock.Object);
        serviceProvider = services.BuildServiceProvider();
        mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        services.Clear();
        serviceProvider.Dispose();
    }

    [Test, Order(1)]
    public async Task Delete_Order_return_success()
    {
        Guid orderId = new("724cbd34-3dff-4e2a-a413-48825f1ab3b9");

        var deleteOrderRequest = new DeleteOrderRequest(orderId);

        var order = new Domain.Order();

        orderRepositoryMock
                .Setup(x => x.GetByIdAsync(orderId))
                .Returns(Task.FromResult(order));

        orderRepositoryMock
                .Setup(x => x.DeleteAsync(order));

        var actualResult = await mediator.Send(deleteOrderRequest);
    }

    [Test, Order(2)]
    public async Task Delete_Order_order_not_found_return_success()
    {
        Guid orderId = new("724cbd34-3dff-4e2a-a413-48825f1ab3b9");

        var deleteOrderRequest = new DeleteOrderRequest(orderId);

        var order = new Domain.Order();

        orderRepositoryMock
                .Setup(x => x.GetByIdAsync(orderId));

        orderRepositoryMock
                .Setup(x => x.DeleteAsync(order));

        var actualResult = await mediator.Send(deleteOrderRequest);
    }
}