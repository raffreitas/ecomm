using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockDepletedDomainEventHandler(ILogger<StockDepletedDomainEventHandler> logger)
    : IDomainEventHandler<StockDepletedDomainEvent>
{
    public Task HandleAsync(StockDepletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogError("[Inventory] Estoque esgotado: Produto {ProductId}.", domainEvent.ProductId);
        // Possível ação: bloquear vendas, notificar comercial, etc.
        return Task.CompletedTask;
    }
}
