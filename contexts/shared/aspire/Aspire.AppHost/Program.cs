var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume();

var postgresql = builder.AddPostgres("postgressql")
    .WithHostPort(5432)
    .WithDataVolume();

var productsDb = postgresql
    .AddDatabase("products");

builder
    .AddProject<Projects.Ecomm_Products_WebApi>("ecomm-products-webapi")
    .WithEnvironment("SemanticKernel__ApiKey", builder.AddParameter("semanticKernel-apiKey", secret: true))
    .WithEnvironment("SemanticKernel__ModelName", builder.AddParameter("semanticKernel-modelName", secret: true))
    .WithReference(productsDb, "DatabaseConnection")
        .WaitFor(postgresql)
    .WithReference(rabbitmq, "MessageBrokerConnection")
        .WaitFor(rabbitmq);

await builder.Build().RunAsync();
