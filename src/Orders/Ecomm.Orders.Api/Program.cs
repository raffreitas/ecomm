using Ecomm.Orders.Api.Extensions;
using Ecomm.Orders.Application;
using Ecomm.Orders.Infrastructure;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

builder.Services.AddApiReference();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapApiReference();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();