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

        public BookingsRepository BookingsRepository { get; }
        public HotelRepository HotelsRepository { get; }
        public RoomsRepository RoomsRepository { get; }
        public IAvisHireCarRepository AvisHireCarRepository { get; }

        public UnitOfWork(
            AzureSqlHbsDbContext azureSqlHbsDbContext, 
            IAvisRentalDataSource avisRentalDataSource,
            ILogger<UnitOfWork> logger)
        {
            _azureSqlHbsDbContext = azureSqlHbsDbContext;
            _avisRentalDataSource = avisRentalDataSource;
            _logger = logger;

            BookingsRepository = new BookingsRepository(_azureSqlHbsDbContext);
            RoomsRepository = new RoomsRepository(_azureSqlHbsDbContext);
            HotelsRepository = new HotelRepository(_azureSqlHbsDbContext);
            AvisHireCarRepository = new AvisHireCarRepository(_avisRentalDataSource);
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
                _logger.LogInformation($"The request was cancelled while saving to the database.");

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
