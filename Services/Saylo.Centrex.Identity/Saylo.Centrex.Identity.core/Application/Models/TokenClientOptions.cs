namespace Saylo.Centrex.Identity.Core.Application.Models;

public class TokenClientOptions
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}