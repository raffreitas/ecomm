using Ecomm.Catalog.Features;
using Ecomm.Catalog.Infrastructure;
using Ecomm.Catalog.Infrastructure.Http.ExceptionHandling;
using Ecomm.ServiceDefaults;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddFeatureValidation()
    .AddFeatureHandlers();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();

app.ApplyMigrations();
app.MapCatalogEndpoints();
app.MapDefaultEndpoints();

await app.RunAsync();
