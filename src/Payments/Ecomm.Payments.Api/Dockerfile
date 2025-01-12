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
COPY ["src/Payments/Ecomm.Payments.Api/Ecomm.Payments.Api.csproj", "src/Payments/Ecomm.Payments.Api/"]
COPY ["src/Payments/Ecomm.Payments.Application/Ecomm.Payments.Application.csproj", "src/Payments/Ecomm.Payments.Application/"]
COPY ["src/Payments/Ecomm.Payments.Domain/Ecomm.Payments.Domain.csproj", "src/Payments/Ecomm.Payments.Domain/"]
COPY ["src/Payments/Ecomm.Payments.Infrastructure/Ecomm.Payments.Infrastructure.csproj", "src/Payments/Ecomm.Payments.Infrastructure/"]
RUN dotnet restore "./src/Payments/Ecomm.Payments.Api/Ecomm.Payments.Api.csproj"
COPY . .
WORKDIR "/src/src/Payments/Ecomm.Payments.Api"
RUN dotnet build "./Ecomm.Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecomm.Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecomm.Payments.Api.dll"]