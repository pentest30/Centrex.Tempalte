using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class FileSystemCertificateStorage : ICertificateStorage
{
    private readonly string _storePath;
    private readonly string _password;
    private readonly ILogger<FileSystemCertificateStorage> _logger;

    public FileSystemCertificateStorage(
        OpenIdCertificateSettings settings,
        ILogger<FileSystemCertificateStorage> logger)
    {
        _storePath = settings.CertificateStorePath;
        _password = settings.CertificateExportPassword;
        _logger = logger;
    }

    public async Task SaveCertificateAsync(string fileName, byte[] certificateData)
    {
        var path = Path.Combine(_storePath, fileName);
        await File.WriteAllBytesAsync(path, certificateData);
    }

    public Task<CertificateInfo> LoadCertificateAsync(CertificateMetadata metadata)
    {
        var path = Path.Combine(_storePath, metadata.Filename);
        var cert = new X509Certificate2(path, _password);

        return Task.FromResult(new CertificateInfo
        {
            Certificate = cert,
            PublicKeyRSA = cert.GetRSAPublicKey(),
            PrivateKeyRSA = cert.GetRSAPrivateKey(),
            Kty = metadata.KeyParameters.Kty,
            Use = metadata.KeyParameters.Use,
            Kid = metadata.Kid,
            N = metadata.KeyParameters.N,
            E = metadata.KeyParameters.E
        });
    }

    public Task DeleteCertificateAsync(string fileName)
    {
        try
        {
            var path = Path.Combine(_storePath, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                _logger.LogInformation("Deleted certificate: {file}", path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting certificate {file}", fileName);
        }

        return Task.CompletedTask;
    }
}