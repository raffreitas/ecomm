using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
