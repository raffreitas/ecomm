using Ecomm.Payments.Application;
using Ecomm.Payments.Infrastructure;
using Ecomm.Payments.Worker;
using Ecomm.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddObservability();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OrderCreatedWorker>();

await builder.Build().RunAsync();
