using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockReplenishedDomainEventHandler(ILogger<StockReplenishedDomainEventHandler> logger)
    : IDomainEventHandler<StockReplenishedDomainEvent>
{
    public Task HandleAsync(StockReplenishedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Inventory] Estoque reabastecido: Produto {ProductId}, Quantidade Atual: {CurrentQuantity}", domainEvent.ProductId, domainEvent.CurrentQuantity);
        // Possível ação: atualizar dashboards, notificar compras, etc.
        return Task.CompletedTask;
    }
}
