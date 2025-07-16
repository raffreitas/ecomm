# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy shared projects first (dependencies)
COPY ["contexts/shared/domain/Ecomm.Shared.SeedWork/Ecomm.Shared.SeedWork.csproj", "contexts/shared/domain/Ecomm.Shared.SeedWork/"]
COPY ["contexts/shared/infrastructure/Ecomm.Shared.Infrastructure/Ecomm.Shared.Infrastructure.csproj", "contexts/shared/infrastructure/Ecomm.Shared.Infrastructure/"]
COPY ["contexts/shared/aspire/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "contexts/shared/aspire/Aspire.ServiceDefaults/"]

# Copy Catalog project files
COPY ["contexts/catalog/src/Ecomm.Catalog.WebApi/Ecomm.Catalog.WebApi.csproj", "contexts/catalog/src/Ecomm.Catalog.WebApi/"]

# Restore dependencies
RUN dotnet restore "contexts/catalog/src/Ecomm.Catalog.WebApi/Ecomm.Catalog.WebApi.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/contexts/catalog/src/Ecomm.Catalog.WebApi"
RUN dotnet build "Ecomm.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecomm.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecomm.Catalog.WebApi.dll"]