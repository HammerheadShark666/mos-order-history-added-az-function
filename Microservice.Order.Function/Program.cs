using Microservice.Order.Function.Data.Contexts;
using Microservice.Order.Function.Data.Repository;
using Microservice.Order.Function.Data.Repository.Interfaces;
using Microservice.Order.Function.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(c =>
    {
        c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
         
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())); 
        services.AddScoped<IOrderRepository, OrderRepository>(); 
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddMemoryCache();

        services.AddDbContextFactory<OrderDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString(Constants.DatabaseConnectionString),
            options => options.EnableRetryOnFailure()));
    })
    .Build();

await host.RunAsync();
