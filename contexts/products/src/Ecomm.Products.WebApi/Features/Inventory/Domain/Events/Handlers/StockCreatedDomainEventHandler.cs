using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class StockCreatedDomainEventHandler(ILogger<StockCreatedDomainEventHandler> logger)
    : IDomainEventHandler<StockCreatedDomainEvent>
{
    public Task HandleAsync(StockCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"[Inventory] Novo estoque criado: Produto {domainEvent.ProductId}, Quantidade Inicial: {domainEvent.InitialQuantity}");
        // Possível ação: notificar sistema externo, enviar e-mail, etc.
        return Task.CompletedTask;
    }
}
