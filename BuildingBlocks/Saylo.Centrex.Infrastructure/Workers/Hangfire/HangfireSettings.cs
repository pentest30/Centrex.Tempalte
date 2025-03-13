
namespace Saylo.Centrex.Infrastructure.Workers.Hangfire;
public class HangfireSettings
{
    /// <summary>
    /// Connection string to the database used by Hangfire.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Path for the Hangfire Dashboard. Default is "/hangfire".
    /// </summary>
    public string? DashboardPath { get; set; } = "/jobs";

    /// <summary>
    /// Username for basic authentication to access the Hangfire Dashboard.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Password for basic authentication to access the Hangfire Dashboard.
    /// </summary>
    public string Password { get; set; }

    public ICollection<HangfireJobConfig>? HangfireJobs { get; set; }
}
public class HangfireJobConfig
{
    public string JobName { get; set; }
    public string CronExpression { get; set; }
}
