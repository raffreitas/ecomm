using Ecomm.Products.WebApi.Features.Inventory.Commands.AddStock;
using Ecomm.Products.WebApi.Features.Inventory.Commands.CreateInventory;
using Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;
using Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;
using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Infrastructure.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoriesPaged;
using Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoryByProductId;
using Ecomm.Products.WebApi.Features.Inventory.Queries.GetLowStockInventories;
using Ecomm.Products.WebApi.Features.Products.Domain.Events;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryFeature(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        services.AddScoped<IDomainEventHandler<ProductCreatedDomainEvent>, CreateItemOnInventoryDomainEventHandler>();

        // Command handlers
        services.AddScoped<CreateInventoryHandler>();
        services.AddScoped<AddStockHandler>();
        services.AddScoped<RemoveStockHandler>();

        // Query handlers
        services.AddScoped<GetInventoryByProductIdHandler>();
        services.AddScoped<GetInventoriesPagedHandler>();
        services.AddScoped<GetLowStockInventoriesHandler>();

        return services;
    }
}
