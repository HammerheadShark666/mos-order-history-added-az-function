using Microservice.Customer.Address.Function.Helpers.Exceptions;
using Microservice.Order.Function.Data.Context;
using Microservice.Order.Function.Data.Repository;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microservice.Order.Function.Domain;
using Microservice.Order.Function.Helpers;
using Microservice.Order.Function.MediatR.DeleteOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Microservice.Order.Function.Test.Unit.Repository;

[TestFixture]
public class OrderRepositoryTests
{
    private Mock<IDbContextFactory<OrderDbContext>> mockDbFactory = new();
    private readonly IOrderRepository orderRepository = default!;
    private readonly Mock<ILogger<DeleteOrderCommandHandler>> loggerMock = new();
    private readonly ServiceCollection services = new();
    private ServiceProvider serviceProvider;

    private readonly Guid orderId = new("724cbd34-3dff-4e2a-a413-48825f1ab3b2");

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        mockDbFactory = CreateInMemoryDbFactory(orderId);
        services.AddScoped(sp => orderRepository);
        services.AddScoped(sp => loggerMock.Object);
        serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        services.Clear();
        serviceProvider.Dispose();
    }

    [Test, Order(1)]
    public async Task Get_order_from_repository_return_order()
    {
        var orderRepository = new OrderRepository(mockDbFactory.Object);
        var result = await orderRepository.GetByIdAsync(orderId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Domain.Order>());
            Assert.That(result.Id, Is.EqualTo(orderId));
            Assert.That(result.OrderItems, Has.Count.EqualTo(1));
            Assert.That(result.OrderItems[0].Name, Is.EqualTo("Terror Wars"));
        });
    }

    [Test, Order(2)]
    public async Task Get_order_from_repository_not_found_return_exception()
    {
        Guid notFoundOrderId = new("724cbd34-3dff-4e2a-a413-48825f1ab3b9");

        var orderRepository = new OrderRepository(mockDbFactory.Object);

        var notFoundException = Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await orderRepository.GetByIdAsync(notFoundOrderId);
        });

        Assert.That(notFoundException.Message, Is.EqualTo(string.Format("Order not found.")));
    }

    [Test, Order(3)]
    public async Task Delete_order_from_repository_return_order()
    {
        var orderRepository = new OrderRepository(mockDbFactory.Object);
        var order = await orderRepository.GetByIdAsync(orderId);

        await orderRepository.DeleteAsync(order);

        var notFoundException = Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            order = await orderRepository.GetByIdAsync(orderId);
        });

        Assert.That(notFoundException.Message, Is.EqualTo(string.Format("Order not found.")));
    }

    private static Mock<IDbContextFactory<OrderDbContext>> CreateInMemoryDbFactory(Guid orderId)
    {
        var mockDbFactory = new Mock<IDbContextFactory<OrderDbContext>>();

        var options = new DbContextOptionsBuilder<OrderDbContext>()
                            .UseInMemoryDatabase(databaseName: "FreeDb")
                            .Options;

        // Insert seed data into the database using an instance of the context
        using (var context = new OrderDbContext(options))
        {
            context.Orders.Add(CreateOrder(orderId));
            context.SaveChanges();
        }

        // Now the in-memory db already has data, we don't have to seed everytime the factory returns the new DbContext:
        mockDbFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => new OrderDbContext(options));

        return mockDbFactory;
    }

    private static Domain.Order CreateOrder(Guid orderId)
    {
        var orderStatus = new OrderStatus() { Id = Enums.OrderStatus.Created, Status = "Created" };

        var productType = new ProductType() { Id = Enums.ProductType.Book, Name = "Book" };

        var orderItem = new OrderItem() { OrderId = orderId, ProductType = productType, Name = "Terror Wars", Quantity = 1, UnitPrice = 8.99m };

        var orderItems = new List<OrderItem>() { orderItem };

        var order = new Domain.Order() { Id = orderId, OrderStatus = orderStatus, OrderItems = orderItems };

        return order;
    }
}