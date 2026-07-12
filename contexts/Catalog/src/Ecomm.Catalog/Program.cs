using Ecomm.Catalog.Features;
using Ecomm.Catalog.Infrastructure;
using Ecomm.Catalog.Infrastructure.Http.ExceptionHandling;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddFeatureValidation();

builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();

app.ApplyMigrations();
app.MapCatalogEndpoints();

app.Run();
