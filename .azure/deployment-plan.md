# Azure Deployment Plan

> **Status:** Planning

Generated: 2026-07-13

## 1. Project Overview

**Goal:** Modernize the existing Ecomm microservices for Azure Container Apps, Azure Service Bus Standard, Azure SQL, Key Vault, ACR and correlated telemetry.

**Path:** Modernize Existing

## 2. Requirements

| Attribute | Value |
|---|---|
| Classification | Development / production-didactic lab |
| Scale | Small, autoscaling APIs 1-3 and workers 0-5 |
| Budget | Cost-optimized |
| Subscription | Not yet confirmed |
| Location | Not yet confirmed |

## 3. Components Detected

| Component | Type | Technology | Path |
|---|---|---|---|
| Catalog | API | ASP.NET Core 10 Minimal API | `contexts/Catalog/src/Ecomm.Catalog` |
| Customers | API | ASP.NET Core 10 Minimal API | `contexts/Customers/Ecomm.Customers.Api` |
| Orders API | API | ASP.NET Core 10 | `contexts/Orders/Ecomm.Orders.Api` |
| Orders Worker | Worker | .NET 10 Worker Service | `contexts/Orders/Ecomm.Orders.Worker` |
| Payments API | API | ASP.NET Core 10 | `contexts/Payments/Ecomm.Payments.Api` |
| Payments Worker | Worker | .NET 10 Worker Service | `contexts/Payments/Ecomm.Payments.Worker` |

## 4. Recipe Selection

**Selected:** Bicep, as explicitly approved in the architectural plan.

## 5. Architecture

**Stack:** Containers

| Component | Azure Service | SKU |
|---|---|---|
| Six application containers | Azure Container Apps | Consumption |
| Images | Azure Container Registry | Basic |
| Event transport | Azure Service Bus | Standard |
| Four isolated databases | Azure SQL logical server | Serverless dev sizing |
| Secrets | Key Vault | Standard |
| Telemetry | Log Analytics + Application Insights | Pay-as-you-go |
| Identity | User-assigned managed identity per app/worker | N/A |

SQL authentication will not be generated. Azure SQL will use Entra-only authentication and contained users.

## 6. Provisioning Limit Checklist

| Resource Type | Number to Deploy | Status |
|---|---:|---|
| Microsoft.App/managedEnvironments | 1 | Awaiting subscription/location quota check |
| Microsoft.App/containerApps | 6 | Awaiting subscription/location quota check |
| Microsoft.ContainerRegistry/registries | 1 | Awaiting subscription/location quota check |
| Microsoft.ServiceBus/namespaces | 1 | Awaiting subscription/location quota check |
| Microsoft.Sql/servers | 1 | Awaiting subscription/location quota check |
| Microsoft.Sql/servers/databases | 4 | Awaiting subscription/location quota check |
| Microsoft.KeyVault/vaults | 1 | Awaiting subscription/location quota check |
| Microsoft.OperationalInsights/workspaces | 1 | Awaiting subscription/location quota check |
| Microsoft.Insights/components | 1 | Awaiting subscription/location quota check |
| Microsoft.ManagedIdentity/userAssignedIdentities | 6 | Awaiting subscription/location quota check |

## 7. Execution Checklist

- [x] Analyze and scan workspace
- [x] Gather requirements supplied in the approved architecture plan
- [x] Select Bicep recipe and Container Apps architecture
- [ ] Confirm Azure subscription and location
- [ ] Check policies and quotas
- [ ] Generate and validate Azure Bicep
- [ ] Set status to Ready for Validation
- [ ] Run azure-validate

## 8. Files

| File | Purpose | Status |
|---|---|---|
| `.azure/deployment-plan.md` | Azure preparation source of truth | In progress |
| `infra/main.bicep` | Azure resources | Blocked on Azure context |
| `azure.yaml` | Optional azd orchestration | Not selected; direct Bicep is authoritative |
| Service Dockerfiles | Container builds | Complete |

## 9. Next Steps

1. Confirm Azure subscription and region.
2. Query Azure Policy and quotas.
3. Generate Entra-only Bicep and validate it with `azure-validate`.
