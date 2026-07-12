using Ecomm.Orders.Api.Extensions;
using Ecomm.Orders.Application;
using Ecomm.Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

builder.Services.AddApiReference();

var app = builder.Build();

app.MapApiReference();
app.ApplyMigrations();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();