namespace Saylo.Centrex.Infrastructure.Cache;

public class RedisConfig
{
    public string InstanceName { get; set; }
    public int DefaultDatabase { get; set; }
    public int ConnectRetry { get; set; }
    public int ConnectTimeout { get; set; }
    public int SyncTimeout { get; set; }
    public int ResponseTimeout { get; set; }
    public bool AllowAdmin { get; set; }
    public bool Ssl { get; set; }
    public bool AbortOnConnectFail { get; set; }
    public string Password { get; set; }
}
public class RedisCacheSettings
{
    public int DefaultExpirationMinutes { get; set; }
    public int SlidingExpirationMinutes { get; set; }
}