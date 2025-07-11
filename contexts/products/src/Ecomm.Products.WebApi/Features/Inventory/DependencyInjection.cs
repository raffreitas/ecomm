using Ecomm.Products.WebApi.Features.Inventory.Application;
using Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;
using Ecomm.Products.WebApi.Features.Inventory.Commands.AddStock;
using Ecomm.Products.WebApi.Features.Inventory.Commands.CreateInventory;
using Ecomm.Products.WebApi.Features.Inventory.Commands.DeactivateInventory;
using Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;
using Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;
using Ecomm.Products.WebApi.Features.Inventory.Commands.ReserveStock;
using Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;
using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;
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

        services.AddScoped<IInventoryAvailabilityService, InventoryAvailabilityService>();


        // Command handlers
        services.AddScoped<CreateInventoryHandler>();
        services.AddScoped<AddStockHandler>();
        services.AddScoped<RemoveStockHandler>();
        services.AddScoped<ReserveStockHandler>();
        services.AddScoped<ReleaseReservedStockHandler>();
        services.AddScoped<ActivateInventoryHandler>();
        services.AddScoped<DeactivateInventoryHandler>();
        services.AddScoped<UpdateInventoryLimitsHandler>();

        // Query handlers
        services.AddScoped<GetInventoryByProductIdHandler>();
        services.AddScoped<GetInventoriesPagedHandler>();
        services.AddScoped<GetLowStockInventoriesHandler>();

        return services;
    }
}
