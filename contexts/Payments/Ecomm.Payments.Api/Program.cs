using Ecomm.Payments.Api.Extensions;
using Ecomm.Payments.Application;
using Ecomm.Payments.Infrastructure;
using Ecomm.ServiceDefaults;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddDefaultApiServices();

var app = builder.Build();
app.UseExceptionHandler();


app.MapOpenApi();
app.MapScalarApiReference();
app.ApplyMigrations();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
