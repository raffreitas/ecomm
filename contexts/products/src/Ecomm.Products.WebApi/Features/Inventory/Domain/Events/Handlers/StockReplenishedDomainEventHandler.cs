using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class StockReplenishedDomainEventHandler(ILogger<StockReplenishedDomainEventHandler> logger) 
    : IDomainEventHandler<StockReplenishedDomainEvent>
{
    public Task HandleAsync(StockReplenishedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"[Inventory] Estoque reabastecido: Produto {domainEvent.ProductId}, Quantidade Atual: {domainEvent.CurrentQuantity}");
        // Possível ação: atualizar dashboards, notificar compras, etc.
        return Task.CompletedTask;
    }
}
