using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Infrastructure.MultiTenancy.Core;
using StackExchange.Redis;

namespace Saylo.Centrex.Infrastructure.Cache;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("Redis:Configuration").Get<RedisConfig>();
        ArgumentNullException.ThrowIfNull(redisConfig);

        var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = { configuration.GetConnectionString("Redis") },
            Password = redisConfig.Password,
            Ssl = false,
            AbortOnConnectFail = false,
            DefaultDatabase = redisConfig.DefaultDatabase,
            ConnectRetry = redisConfig.ConnectRetry,
            ConnectTimeout = redisConfig.ConnectTimeout,
            SyncTimeout = redisConfig.SyncTimeout,
            ResponseTimeout = redisConfig.ResponseTimeout,
            AllowAdmin = redisConfig.AllowAdmin
        });

        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.Configure<RedisCacheSettings>(configuration.GetSection("Redis:CacheSettings"));

        services.AddScoped<ITenantCacheStore, RedisTenantCacheStore>();

        return services;
    }
}