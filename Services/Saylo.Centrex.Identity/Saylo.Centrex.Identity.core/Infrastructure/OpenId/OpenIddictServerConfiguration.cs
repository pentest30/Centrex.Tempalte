using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using Saylo.Centrex.Identity.Core.Application.Interfaces;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class OpenIddictServerConfiguration : IConfigureOptions<OpenIddictServerOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public OpenIddictServerConfiguration(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Configure(OpenIddictServerOptions options)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRotationService = scope.ServiceProvider.GetRequiredService<IKeyRotationService>();

        var currentCert = keyRotationService.GetCurrentCertificateAsync().GetAwaiter().GetResult();
        var previousCert = keyRotationService.GetPreviousCertificateAsync().GetAwaiter().GetResult();

        if (currentCert != null)
        {
            options.SigningCredentials.Add(
                new SigningCredentials(
                    new X509SecurityKey(currentCert.Certificate),
                    SecurityAlgorithms.RsaSha256
                ));
        }

        if (previousCert != null)
        {
            options.SigningCredentials.Add(
                new SigningCredentials(
                    new X509SecurityKey(previousCert.Certificate),
                    SecurityAlgorithms.RsaSha256
                ));
        }
    }
}