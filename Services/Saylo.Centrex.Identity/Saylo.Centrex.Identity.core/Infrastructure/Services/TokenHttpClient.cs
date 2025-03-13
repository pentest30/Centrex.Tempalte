using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saylo.Centrex.Application.Exceptions;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Services;

public class TokenHttpClient : ITokenClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TokenClientOptions _options;
    private readonly ILogger<TokenHttpClient> _logger;

    public TokenHttpClient(
        IHttpClientFactory httpClientFactory,
        IOptions<TokenClientOptions> options,
        ILogger<TokenHttpClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TokenResponse> GetTokenAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient("TokenClient");
        ArgumentNullException.ThrowIfNull(client);
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password,
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["scope"] = _options.Scope
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "connect/token")
            {
                Content = new FormUrlEncodedContent(tokenRequest)
            };

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Token request failed with status {StatusCode}: {Content}", 
                    response.StatusCode, content);
                throw new TokenRequestException($"Token request failed with status {response.StatusCode}");
            }

            return JsonSerializer.Deserialize<TokenResponse>(content);
        }
        catch (Exception ex) when (ex is not TokenRequestException)
        {
            _logger.LogError(ex, "Error occurred while requesting token");
            throw;
        }
    }
}