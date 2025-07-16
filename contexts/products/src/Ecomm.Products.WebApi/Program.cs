using Ecomm.Products.WebApi;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Startup.ConfigureBuilder(builder);
    var app = builder.Build();
    Startup.ConfigureApp(app);
    await app.RunAsync();
}
catch (Exception ex)
{
    using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    var logger = factory.CreateLogger<WebApplication>();
    logger.LogCritical(ex, "An unhandled exception occurred during application startup.");
}
