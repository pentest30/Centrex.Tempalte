using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Common.Interfaces;

namespace Saylo.Centrex.Infrastructure.Workers.Hangfire;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        ArgumentNullException.ThrowIfNull(hangfireSettings);
        services.AddSingleton(hangfireSettings);
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseSqlServerStorage(() =>
                    new SqlConnection
                    {
                        ConnectionString = hangfireSettings.ConnectionString
                    });

        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
        });
        services.AddSingleton<IJobTypeResolver, AssemblyJobTypeResolver>();
        services.AddSingleton<IJobScheduler, JobScheduler>();

        return services;
    }


    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>(); 
        if (hangfireSettings == null) throw new InvalidOperationException("hangfireSettings  are not configured.");
       
        var dashboardPath = hangfireSettings.DashboardPath ?? "/jobs";
        var dashboardUser = hangfireSettings.User;
        var dashboardPassword = hangfireSettings.Password;
        app.UseHangfireDashboard(dashboardPath, new DashboardOptions
        {
            Authorization =
            [
                new DashboardAuthorizationFilter(dashboardUser, dashboardPassword)
            ]
        });

        return app;
    }
}