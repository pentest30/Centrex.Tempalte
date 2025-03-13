using System.Linq.Expressions;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkMerge;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkUpdate;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Common.Identity;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;

namespace Saylo.Centrex.Infrastructure.Persistence
{
    public class DbContextRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>, IAggregateRoot
        where TDbContext : DbContext, IUnitOfWork
    {
        private readonly TDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        private DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        public IUnitOfWork UnitOfWork => _dbContext;

        public DbContextRepository(TDbContext dbContext, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public async Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                await AddAsync(entity, cancellationToken);
            }
            else
            {
                await UpdateAsync(entity, cancellationToken);
            }
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
            entity.CreatedBy = _currentUserService.UserName;
            entity.CreatedById = _currentUserService.UserId;
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public  Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
            entity.UpdateBy = _currentUserService.UserName;
            entity.UpdateById = _currentUserService.UserId;
            return Task.CompletedTask;
        }

        public void Attach(TEntity entity)
        {
            DbSet.Attach(entity);
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void Detach(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public Task<T1> FirstOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.FirstOrDefaultAsync();
        }

        public Task<T1> SingleOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.SingleOrDefaultAsync();
        }

        public Task<List<T1>> ToListAsync<T1>(IQueryable<T1> query)
        {
            return query.ToListAsync();
        }

        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            _dbContext.BulkInsert(entities);
        }

        public void BulkInsert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector)
        {
            _dbContext.BulkInsert(entities, columnNamesSelector);
        }

        public void BulkUpdate(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector)
        {
            _dbContext.BulkUpdate(entities, columnNamesSelector);
        }

        public void BulkDelete(IEnumerable<TEntity> entities)
        {
            _dbContext.BulkDelete(entities);
        }

        public void BulkMerge(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> updateColumnNamesSelector, Expression<Func<TEntity, object>> insertColumnNamesSelector)
        {
            _dbContext.BulkMerge(entities, idSelector, updateColumnNamesSelector, insertColumnNamesSelector);
        }

        public void SetRowVersion(TEntity entity, byte[] version)
        {
            _dbContext.Entry(entity).OriginalValues[nameof(entity.RowVersion)] = version;
        }

        public bool IsDbUpdateConcurrencyException(Exception ex)
        {
            return ex is DbUpdateConcurrencyException;
        }
    }
}
