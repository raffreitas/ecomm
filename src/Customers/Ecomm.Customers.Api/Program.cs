using Ecomm.Customers.Api.Endpoints;
using Ecomm.Customers.Api.Extensions;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddFluentValidation()
    .AddDependencyInjection();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.ApplyMigrations();
}

app.MapCustomerEndpoints();

app.UseHttpsRedirection();

app.Run();