using Ecomm.Payments.Api.Extensions;
using Ecomm.Payments.Application;
using Ecomm.Payments.Infrastructure;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();
app.ApplyMigrations();

app.UseAuthorization();

app.MapControllers();

app.Run();