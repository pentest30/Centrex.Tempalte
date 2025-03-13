using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Infrastructure.Behaviors;
using Saylo.Centrex.Infrastructure.Persistence;
using Saylo.Centrex.Infrastructure.Services;
using Saylo.Centrex.Infrastructure.Services.Outbox;
using Saylo.Centrex.Infrastructure.Web.Filters;
using Saylo.Centrex.Infrastructure.Workers.Hangfire;

namespace Saylo.Centrex.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure<TDbContext>(
        this IServiceCollection services) where TDbContext : DbContext, IUnitOfWork
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IRepository<OutboxEvent, Guid>, DbContextRepository<TDbContext, OutboxEvent, Guid>>();
        services.AddTransient<IJobService, HangfireService>();
        services.AddScoped<OutboxPublisherJob>();
        services.AddSingleton<HangfireJobScheduler>();
        services.AddSingleton<IMessageDeserializer, JsonMessageDeserializer>();
        services.AddScoped<ValidateTenantAttribute>();
        services.AddScoped<IIntegrationEventPublisherService, IntegrationEventPublisherService>();
        services.AddBehaviours();
        return services;
    }
}