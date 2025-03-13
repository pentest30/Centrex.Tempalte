using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class KeyRotationService : IHostedService, IKeyRotationService, IDisposable, IAsyncDisposable
{
    private readonly ILogger<KeyRotationService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICertificateGenerator _certificateGenerator;
    private readonly ICertificateStorage _certificateStorage;
    private readonly OpenIdCertificateSettings _settings;
    private readonly IDateTimeProvider _dateTimeProvider;
    private Timer _timer;
    private bool _disposed;

    public KeyRotationService(
        ILogger<KeyRotationService> logger,
        IServiceScopeFactory scopeFactory,
        ICertificateGenerator certificateGenerator,
        ICertificateStorage certificateStorage,
        OpenIdCertificateSettings settings,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _certificateGenerator = certificateGenerator;
        _certificateStorage = certificateStorage;
        _settings = settings;
        _dateTimeProvider = dateTimeProvider;
    }

    private async Task DoWorkAsync()
    {
        try
        {
            _logger.LogDebug("Starting key rotation check");
            using var scope = _scopeFactory.CreateScope();
            await CheckAndRotateKeysAsync(scope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during key rotation check");
        }
    }

    private async Task CheckAndRotateKeysAsync(IServiceScope scope)
    {
        var repository = scope.ServiceProvider.GetRequiredService<ICertificateMetadataRepository>();
        var current = await repository.GetActiveCertificateAsync();

        if (current == null)
        {
            _logger.LogInformation("No active certificate found. Initiating key rotation");
            await RotateKeysAsync(scope);
        }
        else if (_dateTimeProvider.UtcNow - current.CreatedAt >= TimeSpan.FromMinutes(_settings.KeyRotationPeriodDays))
        {
            _logger.LogInformation("Certificate rotation period exceeded. Current certificate created at: {CreatedAt}", current.CreatedAt);
            await RotateKeysAsync(scope);
        }
        else
        {
            _logger.LogDebug("Certificate rotation not needed. Current certificate age: {Age} minutes",
                (_dateTimeProvider.UtcNow - current.CreatedAt).TotalMinutes);
        }
    }

    public async Task<CertificateInfo> GetCurrentCertificateAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        try
        {
            var repository = scope.ServiceProvider.GetRequiredService<ICertificateMetadataRepository>();
            var metadata = await repository.GetActiveCertificateAsync();

            if (metadata == null)
            {
                _logger.LogInformation("No active certificate found during GetCurrentCertificate. Initiating rotation");
                await RotateKeysAsync(scope);
                metadata = await repository.GetActiveCertificateAsync();
            }

            _logger.LogDebug("Retrieved current certificate. Thumbprint: {Thumbprint}", metadata?.Thumbprint);
        
            try
            {
                return await _certificateStorage.LoadCertificateAsync(metadata);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Certificate file not found at expected location. Initiating emergency rotation");
                // Mark the current certificate as invalid in the repository
                await repository.DeactivateCurrentCertificateAsync();
                // Rotate keys
                await RotateKeysAsync(scope);
                // Get the newly rotated certificate
                metadata = await repository.GetActiveCertificateAsync();
                return await _certificateStorage.LoadCertificateAsync(metadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current certificate");
            throw;
        }
    }

    public async Task<CertificateInfo> GetPreviousCertificateAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        try
        {
            var repository = scope.ServiceProvider.GetRequiredService<ICertificateMetadataRepository>();
            var metadata = await repository.GetPreviousCertificateAsync();

            _logger.LogDebug("Retrieved previous certificate. Thumbprint: {Thumbprint}", metadata?.Thumbprint);
            return metadata != null ? await _certificateStorage.LoadCertificateAsync(metadata) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving previous certificate");
            throw;
        }
    }

    private async Task RotateKeysAsync(IServiceScope scope)
    {
        try
        {
            _logger.LogInformation("Starting key rotation process");
            var repository = scope.ServiceProvider.GetRequiredService<ICertificateMetadataRepository>();

            var (certInfo, exportedCert) = _certificateGenerator.GenerateNewCertificate(_settings.CertificateLifeSpanDays);
            var fileName = $"cert_{_dateTimeProvider.UtcNow:yyyyMMddHHmmss}.pfx";

            _logger.LogInformation("Generated new certificate. Thumbprint: {Thumbprint}", certInfo.Certificate.Thumbprint);
            await _certificateStorage.SaveCertificateAsync(fileName, exportedCert);

            var metadata = new CertificateMetadata
            {
                Id = Guid.NewGuid(),
                Filename = fileName,
                CreatedAt = _dateTimeProvider.UtcNow,
                ExpiresAt = _dateTimeProvider.UtcNow.AddDays(_settings.CertificateLifeSpanDays),
                Thumbprint = certInfo.Certificate.Thumbprint,
                Kid = certInfo.Kid,
                IsActive = true,
                KeyParameters = new KeyParameters
                {
                    Kty = certInfo.Kty,
                    Use = certInfo.Use,
                    N = certInfo.N,
                    E = certInfo.E
                }
            };

            await repository.DeactivateCurrentCertificateAsync();
            await repository.AddAsync(metadata);
            _logger.LogInformation("New certificate activated. ID: {Id}, Expires: {ExpiresAt}", metadata.Id, metadata.ExpiresAt);

            await CleanupOldCertificatesAsync(scope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during key rotation");
            throw;
        }
    }

    private async Task CleanupOldCertificatesAsync(IServiceScope scope)
    {
        try
        {
            _logger.LogInformation("Starting cleanup of old certificates");
            var repository = scope.ServiceProvider.GetRequiredService<ICertificateMetadataRepository>();
            var oldCerts = await repository.GetExpiredCertificatesAsync(keepCount: 2);

            foreach (var cert in oldCerts)
            {
                _logger.LogInformation("Deleting expired certificate. ID: {Id}, Created: {CreatedAt}", cert.Id, cert.CreatedAt);
                await _certificateStorage.DeleteCertificateAsync(cert.Filename);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during certificate cleanup");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting KeyRotationService");
        _ = DoWorkAsync();

        _timer = new Timer(
            async _ => await DoWorkAsync(),
            null,
            TimeSpan.FromDays(_settings.KeyRotationPeriodDays),
            TimeSpan.FromDays(_settings.KeyRotationPeriodDays)
        );

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping KeyRotationService");
        if (_timer != null)
        {
            await _timer.DisposeAsync();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }
            Dispose(false);
        }
    }
}