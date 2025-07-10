using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class StockBelowMinimumDomainEventHandler(ILogger<StockBelowMinimumDomainEventHandler> logger)
    : IDomainEventHandler<StockBelowMinimumDomainEvent>
{
    public Task HandleAsync(StockBelowMinimumDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogWarning($"[Inventory] Estoque abaixo do mínimo: Produto {domainEvent.ProductId}, Quantidade Atual: {domainEvent.CurrentQuantity}, Mínimo: {domainEvent.MinimumStockLevel}");
        // Possível ação: disparar alerta, abrir chamado de compra, etc.
        return Task.CompletedTask;
    }
}
