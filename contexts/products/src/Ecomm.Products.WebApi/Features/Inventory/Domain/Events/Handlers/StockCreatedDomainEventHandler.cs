using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class StockCreatedDomainEventHandler(ILogger<StockCreatedDomainEventHandler> logger)
    : IDomainEventHandler<StockCreatedDomainEvent>
{
    public Task HandleAsync(StockCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Inventory] Novo estoque criado: Produto {ProductId}, Quantidade Inicial: {InitialQuantity}", domainEvent.ProductId, domainEvent.InitialQuantity);
        // Possível ação: notificar sistema externo, enviar e-mail, etc.
        return Task.CompletedTask;
    }
}
