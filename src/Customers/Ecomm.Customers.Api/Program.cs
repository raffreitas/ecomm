using System.Reflection;

using Ecomm.Customers.Api.Endpoints;
using Ecomm.Customers.Api.Messaging;
using Ecomm.Customers.Api.Persistence;
using Ecomm.Customers.Api.Persistence.Repositories;
using Ecomm.Customers.Api.Repositories;
using Ecomm.Customers.Api.Services;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomersDbContext>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IMessageBusService, RabbitMqMessageBusService>();

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCustomerEndpoints();

app.UseHttpsRedirection();

app.Run();