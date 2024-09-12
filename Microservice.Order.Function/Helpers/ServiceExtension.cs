using Microservice.Order.Function.Data.Context;
using Microservice.Order.Function.Data.Repository;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microservice.Order.Function.Helpers;

public static class ServiceExtension
{
    public static void ConfigureApplicationInsights(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    }

    public static void ConfigureDependencyInjection(IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    public static void ConfigureMediatr(IServiceCollection services)
    {
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static void ConfigureMemoryCache(IServiceCollection services)
    {
        services.AddMemoryCache();
    }

    public static void ConfigureSqlServer(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<OrderDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(Constants.DatabaseConnectionString),
                options => options.EnableRetryOnFailure()));
    }
}