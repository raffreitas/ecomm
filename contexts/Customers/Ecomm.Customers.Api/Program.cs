using Ecomm.Customers.Api.Endpoints;
using Ecomm.Customers.Api.Extensions;
using Ecomm.ServiceDefaults;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

builder.Services
    .AddDatabase(builder.Configuration)
    .AddFluentValidation()
    .AddDependencyInjection();

builder.Services.AddOpenApi();
builder.Services.AddDefaultApiServices();

var app = builder.Build();
app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();
app.ApplyMigrations();

app.MapCustomerEndpoints();
app.MapDefaultEndpoints();

app.Run();
