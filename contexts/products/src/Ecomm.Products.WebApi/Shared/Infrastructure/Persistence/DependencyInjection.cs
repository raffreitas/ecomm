using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Extensions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Shared;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence;

public static class DependencyInjection
{
    public const string ConnectionStringName = "DatabaseConnection";

    public static IServiceCollection AddPersistenceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static WebApplication UsePersistenceConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();
        }

        return app;
    }
}
