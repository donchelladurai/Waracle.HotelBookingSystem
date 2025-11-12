using Microsoft.EntityFrameworkCore;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;

namespace Waracle.HotelBookingSystem.Data.Repositories
{
    public abstract class GenericSqlRepository<T> : IGenericSqlRepository<T> where T : class
    {
        protected readonly AzureSqlHbsDbContext _azureSqlDbContext;
        private readonly DbSet<T> _dbSet;

        public GenericSqlRepository(AzureSqlHbsDbContext azureSqlDbContext)
        {
            _azureSqlDbContext = azureSqlDbContext;
            _dbSet = _azureSqlDbContext.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                return await _dbSet.FindAsync(id, cancellationToken);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                throw operationCanceledException;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await _dbSet.ToListAsync(cancellationToken);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                throw operationCanceledException;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _dbSet.AddAsync(entity, cancellationToken);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                throw operationCanceledException;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task Update(T entity, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _dbSet.Attach(entity);
                _azureSqlDbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (OperationCanceledException operationCanceledException)
            {
                throw operationCanceledException;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task Delete(T entity, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_azureSqlDbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }

                _dbSet.Remove(entity);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                throw operationCanceledException;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
