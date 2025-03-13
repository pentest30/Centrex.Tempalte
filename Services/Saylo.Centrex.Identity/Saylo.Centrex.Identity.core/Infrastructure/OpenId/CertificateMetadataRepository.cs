using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class EfCertificateMetadataRepository(IRepository<CertificateMetadataEntity, Guid> repository,
    IDateTimeProvider dateTimeProvider,
    ILogger<EfCertificateMetadataRepository> logger)
    : ICertificateMetadataRepository
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<EfCertificateMetadataRepository> _logger = logger;

    public async Task AddAsync(CertificateMetadata metadata)
    {
        var entity = new CertificateMetadataEntity
        {
            Id = metadata.Id,
            Filename = metadata.Filename,
            CreatedAt = metadata.CreatedAt,
            ExpiresAt = metadata.ExpiresAt,
            Thumbprint = metadata.Thumbprint,
            Kid = metadata.Kid,
            IsActive = metadata.IsActive,
            Kty = metadata.KeyParameters.Kty,
            Use = metadata.KeyParameters.Use,
            N = metadata.KeyParameters.N,
            E = metadata.KeyParameters.E
        };

        await repository.AddAsync(entity);
        await repository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<CertificateMetadata?> GetActiveCertificateAsync()
    {
        var now = _dateTimeProvider.UtcNow;
        var query = repository.GetAll()
            .Where(x => x.IsActive && x.ExpiresAt > now);

        var entity = await query
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            _logger.LogWarning("No valid active certificate found. Current time: {Now}", now);
        }

        return entity != null ? MapToMetadata(entity) : null;
    }

    public async Task<CertificateMetadata?> GetPreviousCertificateAsync()
    {
        var entity = await repository.GetAll()
            .Where(x => !x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        return entity != null ? MapToMetadata(entity) : null;
    }

    public async Task DeactivateCurrentCertificateAsync()
    {
        var current = await repository.GetAll()
            .Where(x => x.IsActive)
            .ToListAsync();

        foreach (var cert in current)
        {
            cert.IsActive = false;
        }

        await repository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<CertificateMetadata>> GetExpiredCertificatesAsync(int keepCount)
    {
        var certificates = await repository.GetAll()
            .OrderByDescending(x => x.CreatedAt)
            .Skip(keepCount)
            .ToListAsync();

        return certificates.Select(MapToMetadata);
    }

    private CertificateMetadata MapToMetadata(CertificateMetadataEntity entity)
    {
        return new CertificateMetadata
        {
            Id = entity.Id,
            Filename = entity.Filename,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            Thumbprint = entity.Thumbprint,
            Kid = entity.Kid,
            IsActive = entity.IsActive,
            KeyParameters = new KeyParameters
            {
                Kty = entity.Kty,
                Use = entity.Use,
                N = entity.N,
                E = entity.E
            }
        };
    }
}