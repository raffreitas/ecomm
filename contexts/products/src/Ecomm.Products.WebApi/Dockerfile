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

# Copy Products project files
COPY ["contexts/products/src/Ecomm.Products.WebApi/Ecomm.Products.WebApi.csproj", "contexts/products/src/Ecomm.Products.WebApi/"]
COPY ["contexts/products/tests/Ecomm.Products.WebApi.UnitTests/Ecomm.Products.WebApi.UnitTests.csproj", "contexts/products/tests/Ecomm.Products.WebApi.UnitTests/"]

# Restore dependencies
RUN dotnet restore "contexts/products/src/Ecomm.Products.WebApi/Ecomm.Products.WebApi.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/contexts/products/src/Ecomm.Products.WebApi"
RUN dotnet build "Ecomm.Products.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecomm.Products.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecomm.Products.WebApi.dll"]