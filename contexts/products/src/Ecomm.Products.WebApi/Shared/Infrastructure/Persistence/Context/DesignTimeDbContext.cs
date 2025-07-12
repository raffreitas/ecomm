using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

public class DesignTimeDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
           .AddEnvironmentVariables()
           .AddUserSecrets<DesignTimeDbContext>()
           .Build();

        // Get the connection string by name
        var connectionString = configuration.GetConnectionString("DatabaseConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder
            .UseNpgsql(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
            .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
