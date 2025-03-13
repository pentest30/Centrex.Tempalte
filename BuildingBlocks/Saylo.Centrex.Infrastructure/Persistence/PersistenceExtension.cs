using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Common.Automapper;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Infrastructure.Persistence;

public static class PersistenceExtension
{
    public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        var assembly = Assembly.GetCallingAssembly();
        var repositoryType = typeof(IRepository<,>);
        var dbContextRepositoryType = typeof(DbContextRepository<,,>);
        var entityOpenGenericType = typeof(Entity<>);

        var aggregateTypes = assembly.GetTypes()
            .Where(type => type.IsClass && 
                           !type.IsAbstract && 
                           typeof(IAggregateRoot).IsAssignableFrom(type))
            .ToList();

        foreach (var aggregateType in aggregateTypes)
        {
            var idType = aggregateType.GetProperty("Id")?.PropertyType;
            if (idType == null) continue;

            // Check if type inherits from Entity<> by walking up the inheritance chain
            var isEntity = IsSubclassOfGeneric(aggregateType, entityOpenGenericType);
            if (!isEntity) continue;

            var interfaceType = repositoryType.MakeGenericType(aggregateType, idType);
            var implementationType = dbContextRepositoryType.MakeGenericType(typeof(TDbContext), aggregateType, idType);
            services.AddScoped(interfaceType, implementationType);
        }

        return services;
    }

    public static IServiceCollection AddReadRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var aggregateRootTypes = callingAssembly
            .GetTypes()
            .Where(type => 
                !type.IsAbstract && 
                !type.IsInterface &&
                typeof(IAggregateRoot).IsAssignableFrom(type))
            .Distinct()
            .ToList();

        foreach (var entityType in aggregateRootTypes)
        {
            var repositoryType = typeof(DbContextReadRepository<,>)
                .MakeGenericType(typeof(TDbContext), entityType);
            var interfaceType = typeof(IReadRepository<>)
                .MakeGenericType(entityType);

            services.AddScoped(interfaceType, repositoryType);
        }

        return services;
    }
    private static bool IsSubclassOfGeneric(Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) return true;
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}