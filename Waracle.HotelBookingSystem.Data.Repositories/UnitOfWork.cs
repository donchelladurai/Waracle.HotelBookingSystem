using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Infrastructure.AvisCarHire.Interfaces;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AzureSqlHbsDbContext _azureSqlHbsDbContext;
        private readonly IAvisRentalDataSource _avisRentalDataSource;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _isDisposed = false;

        public IBookingsRepository BookingsRepository
        {
            get;
            set;
        }

        public IHotelsRepository HotelsRepository
        {
            get;
            set;
        }

        public IRoomsRepository RoomsRepository
        {
            get;
            set;
        }

        public IAvisHireCarRepository AvisHireCarRepository
        {
            get;
            set;
        }

        public UnitOfWork(
            AzureSqlHbsDbContext azureSqlHbsDbContext, 
            IAvisRentalDataSource avisRentalDataSource,
            ILogger<UnitOfWork> logger)
        {
            this._azureSqlHbsDbContext = azureSqlHbsDbContext;
            this._avisRentalDataSource = avisRentalDataSource;
            this._logger = logger;

            BookingsRepository = new BookingsRepository(this._azureSqlHbsDbContext);
            RoomsRepository = new RoomsRepository(this._azureSqlHbsDbContext);
            HotelsRepository = new HotelRepository(this._azureSqlHbsDbContext);
            AvisHireCarRepository = new AvisHireCarRepository(this._avisRentalDataSource);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await _azureSqlHbsDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                this._logger.LogInformation($"The request was cancelled while saving to the database.");

                throw operationCanceledException;
            }
            catch (SqlException sqlException)
            {
                _logger.LogError($"A SQL exception was thrown while saving to the database: {sqlException.Message}");

                throw sqlException;
            }
            catch (Exception e)
            {
                _logger.LogError($"An unexpected exception was thrown while saving to the database: {e.Message}");

                throw e;
            }
        }

        public void Dispose()
        {
            if (_isDisposed is false)
            {
                _azureSqlHbsDbContext.Dispose();
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
    }
}
