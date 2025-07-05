var builder = DistributedApplication.CreateBuilder(args);

var postgresql = builder.AddPostgres("postgressql")
    .WithDataVolume();

var productsDb = postgresql
    .AddDatabase("products");

builder
    .AddProject<Projects.Ecomm_Products_WebApi>("ecomm-products-webapi")
    .WithReference(productsDb, "DatabaseConnection")
    .WaitFor(postgresql);

await builder.Build().RunAsync();
