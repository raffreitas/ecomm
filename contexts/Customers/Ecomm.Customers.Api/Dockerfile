FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "contexts/Customers/Ecomm.Customers.Api/Ecomm.Customers.Api.csproj" -p:NuGetAudit=false
RUN dotnet publish "contexts/Customers/Ecomm.Customers.Api/Ecomm.Customers.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ecomm.Customers.Api.dll"]
