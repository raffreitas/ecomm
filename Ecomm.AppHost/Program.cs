var builder = DistributedApplication.CreateBuilder(args);

const string sqlPassword = "ChangeIt123!";
const string serviceBusConnection = "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var configPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "infra", "servicebus", "Config.json"));
var asaasApiKey = builder.AddParameter("asaas-api-key", secret: true);

var serviceBusSql = builder.AddContainer("servicebus-sql", "mcr.microsoft.com/mssql/server", "2022-latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_SA_PASSWORD", sqlPassword);
var serviceBus = builder.AddContainer("servicebus-emulator", "mcr.microsoft.com/azure-messaging/servicebus-emulator", "latest")
    .WithBindMount(configPath, "/ServiceBus_Emulator/ConfigFiles/Config.json", isReadOnly: true)
    .WithEnvironment("SQL_SERVER", serviceBusSql.Resource.Name)
    .WithEnvironment("MSSQL_SA_PASSWORD", sqlPassword)
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("EMULATOR_HTTP_PORT", "5300")
    .WithEndpoint(port: 5672, targetPort: 5672, name: "amqp")
    .WithHttpEndpoint(port: 5300, targetPort: 5300, name: "health")
    .WaitFor(serviceBusSql);

var catalogSql = builder.AddContainer("catalog-database", "mcr.microsoft.com/mssql/server", "2022-latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_SA_PASSWORD", sqlPassword)
    .WithEndpoint(port: 1433, targetPort: 1433, name: "sql");
var customersDb = AddPostgres("customers-database", 5435, "customers");
var ordersDb = AddPostgres("orders-database", 5433, "orders");
var paymentsDb = AddPostgres("payments-database", 5434, "payments");

builder.AddProject<Projects.Ecomm_Catalog>("catalog-api")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", $"Server=localhost,1433;Database=catalog;User Id=sa;Password={sqlPassword};TrustServerCertificate=True")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WaitFor(catalogSql).WaitFor(serviceBus);
builder.AddProject<Projects.Ecomm_Customers_Api>("customers-api")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", "Host=localhost;Port=5435;Username=postgres;Password=postgres;Database=customers")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WaitFor(customersDb).WaitFor(serviceBus);
builder.AddProject<Projects.Ecomm_Orders_Api>("orders-api")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", "Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=orders")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WaitFor(ordersDb).WaitFor(serviceBus);
builder.AddProject<Projects.Ecomm_Orders_Worker>("orders-worker")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", "Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=orders")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WaitFor(ordersDb).WaitFor(serviceBus);
builder.AddProject<Projects.Ecomm_Payments_Api>("payments-api")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", "Host=localhost;Port=5434;Username=postgres;Password=postgres;Database=payments")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WithEnvironment("Payments__BaseUrl", "https://api-sandbox.asaas.com/")
    .WithEnvironment("Payments__ApiKey", asaasApiKey)
    .WaitFor(paymentsDb).WaitFor(serviceBus);
builder.AddProject<Projects.Ecomm_Payments_Worker>("payments-worker")
    .WithEnvironment("ConnectionStrings__DatabaseConnection", "Host=localhost;Port=5434;Username=postgres;Password=postgres;Database=payments")
    .WithEnvironment("Messaging__ServiceBus__ConnectionString", serviceBusConnection)
    .WithEnvironment("Payments__BaseUrl", "https://api-sandbox.asaas.com/")
    .WithEnvironment("Payments__ApiKey", asaasApiKey)
    .WaitFor(paymentsDb).WaitFor(serviceBus);

builder.Build().Run();

IResourceBuilder<ContainerResource> AddPostgres(string name, int port, string database) =>
    builder.AddContainer(name, "postgres", "17-alpine")
        .WithEnvironment("POSTGRES_DB", database)
        .WithEnvironment("POSTGRES_USER", "postgres")
        .WithEnvironment("POSTGRES_PASSWORD", "postgres")
        .WithEndpoint(port: port, targetPort: 5432, name: "postgres");
