FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "contexts/Payments/Ecomm.Payments.Worker/Ecomm.Payments.Worker.csproj" -p:NuGetAudit=false
RUN dotnet publish "contexts/Payments/Ecomm.Payments.Worker/Ecomm.Payments.Worker.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ecomm.Payments.Worker.dll"]
