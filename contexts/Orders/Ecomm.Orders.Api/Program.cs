using Ecomm.Orders.Api.Extensions;
using Ecomm.Orders.Application;
using Ecomm.Orders.Infrastructure;
using Ecomm.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

builder.Services.AddApiReference();
builder.Services.AddDefaultApiServices();

var app = builder.Build();
app.UseExceptionHandler();

app.MapApiReference();
app.ApplyMigrations();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
