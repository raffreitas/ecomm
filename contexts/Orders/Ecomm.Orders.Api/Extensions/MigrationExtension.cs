using Ecomm.Orders.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Api.Extensions;

public static class MigrationExtension
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<OrdersDbContext>();
        dbContext.Database.Migrate();
    }
}