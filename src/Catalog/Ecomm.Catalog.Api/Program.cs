using Ecomm.Catalog.Api.Exceptions;
using Ecomm.Catalog.Api.Extensions;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddMessageBus()
    .AddServices();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.ApplyMigrations();

app.MapEndpoints();

app.Run();