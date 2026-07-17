# Ecomm

Laboratório de e-commerce em .NET 10 com quatro bounded contexts: Catalog, Customers, Orders e Payments.

## Arquitetura

- Catalog e Customers usam vertical slices em um único projeto por serviço.
- Orders e Payments mantêm Domain, Application, Infrastructure e API; seus consumers executam em workers separados.
- Eventos de domínio permanecem internos. Eventos de integração usam um envelope versionado com `Id`, `Type`, `Version`, `OccurredAtUtc`, `CorrelationId`, `CausationId` e `Data`.
- Cada publicação é gravada em `OutboxMessages` junto com a mudança de negócio. Consumers registram `(MessageId, ConsumerName)` em `InboxMessages` na mesma transação do efeito local.
- Azure Service Bus usa peek-lock, conclusão manual, retry por abandono e DLQ para mensagens inválidas.

### Eventos e topologia

| Evento | Tópico | Subscription consumidora |
|---|---|---|
| `catalog.product-created.v1` | `catalog-events` | `orders-product-projection` |
| `customers.customer-created.v1` | `customers-events` | `orders-customer-projection` |
| `orders.order-created.v1` | `orders-events` | `payments-order-processing` |
| `payments.payment-approved.v1` / `payments.payment-rejected.v1` | `payments-events` | `orders-payment-status` |

## Execução local

Pré-requisitos: Docker Desktop e .NET SDK 10.

### Aspire

```powershell
dotnet user-secrets --project Ecomm.AppHost/Ecomm.AppHost.csproj set "Parameters:asaas-api-key" "sua-chave-sandbox"
dotnet run --project Ecomm.AppHost/Ecomm.AppHost.csproj
```

O AppHost injeta a chave do Asaas como parâmetro secreto e inicia os bancos, o emulador do Service Bus, quatro APIs e dois workers.

### Docker Compose

Defina a chave sandbox do Asaas e suba o ambiente:

```powershell
$env:PAYMENTS_API_KEY = "sua-chave-sandbox"
docker compose up --build
```

O emulador é somente para desenvolvimento/testes. Sua configuração declarativa está em `infra/servicebus/Config.json`.

| Serviço | URL |
|---|---|
| Catalog | http://localhost:8080/scalar/v1 |
| Customers | http://localhost:8081/scalar/v1 |
| Orders | http://localhost:8082/scalar/v1 |
| Payments | http://localhost:8083/scalar/v1 |
| Service Bus health | http://localhost:5300/health |

Todas as APIs expõem `/health` e `/ready`.

## Build e testes

As versões NuGet são centralizadas em `Directory.Packages.props`.

```powershell
dotnet build Ecomm.slnx -m:1
dotnet test Ecomm.slnx -m:1
```

Se o build agregado ficar preso no compilador local, execute os projetos individualmente com `-m:1`.

## Persistência

Catalog usa SQL Server. Customers, Orders e Payments ainda usam PostgreSQL no ambiente local atual; a migração coordenada para quatro bancos Azure SQL isolados faz parte da etapa cloud e requer recriar/validar as migrations para o provider SQL Server antes do cutover.

Migrations novas incluem outbox/inbox, correlação, índice único de pagamento por `OrderId`, ID externo e motivo de rejeição.

## Azure

O destino aprovado é Azure Container Apps, ACR, Service Bus Standard, Key Vault, Log Analytics, Application Insights e um servidor lógico Azure SQL com quatro bancos. O plano de preparação está em `.azure/deployment-plan.md`.

A geração do Bicep permanece bloqueada até confirmar subscription e região, permitindo validar políticas e quotas antes de escolher SKUs e capacidade. Nenhuma implantação Azure é executada automaticamente.
