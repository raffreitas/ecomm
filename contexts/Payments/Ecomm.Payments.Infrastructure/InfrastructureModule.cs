using Ecomm.Messaging;
using Ecomm.Payments.Application.Abstractions;
using Ecomm.Payments.Infrastructure.Payments.Services;
using Ecomm.Payments.Infrastructure.Payments.Settings;
using Ecomm.Payments.Infrastructure.Persistence;
using Ecomm.Payments.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Payments.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        services.AddServiceBusTransport(configuration);
        services.AddSingleton<IOutboxStore, EfOutboxStore<PaymentsDbContext>>();
        services.AddHostedService<OutboxDispatcher>();

        services.Configure<PaymentSettings>(configuration.GetSection("Payments"))
            .AddOptionsWithValidateOnStart<PaymentSettings>();
        services.AddHttpClient<IPaymentService, AsassPaymentService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaymentSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("access_token", settings.ApiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "Ecomm.Payments");
        });

        return services;
    }
}
