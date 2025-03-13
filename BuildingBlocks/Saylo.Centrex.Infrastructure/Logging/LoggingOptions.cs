namespace Saylo.Centrex.Infrastructure.Logging;

public class LoggingOptions
{
    public Dictionary<string, string> LogLevel { get; set; }

    public FileOptions File { get; set; }
}