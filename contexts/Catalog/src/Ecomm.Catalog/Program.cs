using Ecomm.Catalog.Features;
using Ecomm.Catalog.Infrastructure;
using Ecomm.Catalog.Infrastructure.Http.ExceptionHandling;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddFeatureValidation()
    .AddFeatureHandlers();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();

app.ApplyMigrations();
app.MapCatalogEndpoints();

await app.RunAsync();
