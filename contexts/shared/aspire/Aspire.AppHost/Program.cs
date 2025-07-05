var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder
    .AddMongoDB("mongodb")
    .WithDataVolume()
    .AddDatabase("products-db");

builder
    .AddProject<Projects.Ecomm_Products_WebApi>("ecomm-products-webapi")
    .WithReference(mongodb)
    .WaitFor(mongodb);

await builder.Build().RunAsync();
