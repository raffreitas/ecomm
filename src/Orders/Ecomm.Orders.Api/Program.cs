using Ecomm.Orders.Application;
using Ecomm.Orders.Infrastructure;
using Ecomm.Orders.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();