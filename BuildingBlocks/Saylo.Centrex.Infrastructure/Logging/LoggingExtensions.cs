using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Saylo.Centrex.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static IWebHostBuilder UseFileLogger(this IWebHostBuilder builder, IConfiguration config)
        {
            var options = config.GetSection(nameof(LoggingOptions)).Get<LoggingOptions>();

            builder.ConfigureLogging((context, logging) =>
            {
                // Set up logging only with Serilog and file sink
                var logsPath = Path.Combine(context.HostingEnvironment.ContentRootPath, "logs");
                Directory.CreateDirectory(logsPath);

                var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .Enrich.With<ActivityEnricher>()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithProperty("Assembly", assemblyName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .Filter.ByIncludingOnly((logEvent) =>
                    {
                        if (logEvent.Level >= options.File.MinimumLogEventLevel)
                        {
                            var sourceContext = logEvent.Properties.ContainsKey("SourceContext")
                                ? logEvent.Properties["SourceContext"].ToString()
                                : null;

                            var logLevel = GetLogLevel(sourceContext, options);
                            return logEvent.Level >= logLevel;
                        }

                        return false;
                    })
                    .WriteTo.File(
                        Path.Combine(logsPath, "log.txt"),
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB file size
                        rollOnFileSizeLimit: true,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();

                logging.AddSerilog(); // Use Serilog for logging
            });

            return builder;
        }

        private static LogEventLevel GetLogLevel(string context, LoggingOptions options)
        {
            context = context.Replace("\"", string.Empty);
            string level = "Default";
            var matches = options.LogLevel.Keys.Where(k => context.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            if (matches.Any())
            {
                level = matches.Max();
            }

            return (LogEventLevel)Enum.Parse(typeof(LogEventLevel), options.LogLevel[level], true);
        }
    }
}