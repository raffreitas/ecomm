using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class StockDepletedDomainEventHandler(ILogger<StockDepletedDomainEventHandler> logger) 
    : IDomainEventHandler<StockDepletedDomainEvent>
{
    public Task HandleAsync(StockDepletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogError($"[Inventory] Estoque esgotado: Produto {domainEvent.ProductId}.");
        // Possível ação: bloquear vendas, notificar comercial, etc.
        return Task.CompletedTask;
    }
}
