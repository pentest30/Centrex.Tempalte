using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Core;

public class TenantStore<TTenant, TContext>(IServiceProvider serviceProvider) : ITenantStore<TTenant, TContext>
    where TTenant : TenantInfoBase
    where TContext : DbContext
{
    private DbContext GetDbContext()
    {
        var dbContext = serviceProvider.GetRequiredService<TContext>();
        return dbContext;
    }
    public Task<string?> GetConnectionStringForTenantAsync(string tenantId)
    {
        throw new NotImplementedException();
    }
    public async Task<TTenant?> GetTenantAsync(string? tenantId)
    {
        var dbContext = GetDbContext();
        var id = Guid.Parse(tenantId);
        return await dbContext
            .Set<TTenant>()
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t=> t.Id == id && t.IsActive);
    }
}