using Microservice.Order.Function.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(c =>
    {
        c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>()
                              ?? throw new Exception("Configuration not created.");

        var builder = WebApplication.CreateBuilder(args);
        var environment = builder.Environment;

        ServiceExtension.ConfigureApplicationInsights(services);
        ServiceExtension.ConfigureMediatr(services);
        ServiceExtension.ConfigureDependencyInjection(services);
        ServiceExtension.ConfigureMemoryCache(services);
        ServiceExtension.ConfigureSqlServer(services, builder.Configuration, environment);
    })
    .Build();

await host.RunAsync();