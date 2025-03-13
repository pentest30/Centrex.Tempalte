using Saylo.Centrex.Identity.Core.Application.Models;

namespace Saylo.Centrex.Identity.Core.Application.Interfaces;

public interface ITokenClient
{
    Task<TokenResponse> GetTokenAsync(string username, string password);
}