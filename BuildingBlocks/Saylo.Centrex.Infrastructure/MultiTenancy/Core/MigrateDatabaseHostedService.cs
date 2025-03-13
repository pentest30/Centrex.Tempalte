﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Core;

public class MigrateDatabaseHostedService<TContext> : IHostedService
    where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public MigrateDatabaseHostedService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

     public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
