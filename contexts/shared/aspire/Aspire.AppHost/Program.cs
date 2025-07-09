var builder = DistributedApplication.CreateBuilder(args);

var postgresql = builder.AddPostgres("postgressql")
    .WithHostPort(5432)
    .WithDataVolume();

var productsDb = postgresql
    .AddDatabase("products");

builder
    .AddProject<Projects.Ecomm_Products_WebApi>("ecomm-products-webapi")
    .WithReference(productsDb, "DatabaseConnection")
    .WithEnvironment("SemanticKernel__ApiKey", builder.AddParameter("semanticKernel-apiKey", secret: true))
    .WithEnvironment("SemanticKernel__ModelName", builder.AddParameter("semanticKernel-modelName", secret: true))
    .WaitFor(postgresql);

await builder.Build().RunAsync();
