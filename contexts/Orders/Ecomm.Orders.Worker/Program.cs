using Ecomm.Orders.Infrastructure;
using Ecomm.Orders.Worker;
using Ecomm.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddObservability();
builder.Services.AddWorkerInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OrdersEventsWorker>();

await builder.Build().RunAsync();
