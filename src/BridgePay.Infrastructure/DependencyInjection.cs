namespace BridgePay.Infrastructure;

using System;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Domain.Interfaces;
using BridgePay.Infrastructure.ExternalServices.MockBanks;
using BridgePay.Infrastructure.Messaging.Consumers;
using BridgePay.Infrastructure.Messaging.Publishers;
using BridgePay.Infrastructure.Persistence.MongoDB;
using BridgePay.Infrastructure.Persistence.MongoDB.Repositories;
using BridgePay.Infrastructure.Persistence.Postgres;
using BridgePay.Infrastructure.Persistence.Postgres.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service registration extension methods for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all database contexts, repository implementations, messaging components, mock banks, and infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. PostgreSQL (EF Core) via Supabase
        var dbConnectionString = configuration.GetConnectionString("Supabase") 
                                 ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbConnectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // 2. MongoDB (Audit Log & Webhooks)
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IAuditLogService, AuditLogRepository>();
        services.AddScoped<WebhookPayloadRepository>();

        // 3. PostgreSQL Repositories & Unit of Work
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IMerchantRepository, MerchantRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 4. Mock Bank APIs (Bridge Pattern Implementors)
        services.AddTransient<MockStandardBankApi>();
        services.AddTransient<MockFnbApi>();
        services.AddTransient<MockAbsaBankApi>();
        services.AddSingleton<BankApiFactory>();

        // 5. MassTransit & RabbitMQ Setup
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TransactionConsumer>();
            x.AddConsumer<RefundConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitUri = configuration.GetValue<string>("RabbitMQ:Uri") ?? "rabbitmq://localhost";
                cfg.Host(new Uri(rabbitUri));
                cfg.ConfigureEndpoints(context);
            });
        });

        // Register custom event publisher wrapping MassTransit publish
        services.AddScoped<IMessagePublisher, TransactionPublisher>();

        return services;
    }
}
