using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Data.Repositories
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly AzureSqlHbsDbContext _azureSqlHbsDbContext;

        /// <summary>
        /// A repository for CRUD operations related to rooms
        /// </summary>
        /// <param name="azureSqlHbsDbContext">The Azure SQL DB Context</param>
        public RoomsRepository(AzureSqlHbsDbContext azureSqlHbsDbContext)
        {
            _azureSqlHbsDbContext = azureSqlHbsDbContext;
        }

        /// <summary>
        /// Returns a list of rooms for a specific hotel
        /// </summary>
        /// <param name="hotelId">The hotel Id</param>
        /// <returns>A list of rooms for a specific hotel</returns>
        public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return await _azureSqlHbsDbContext.Rooms
                             .Include(r => r.RoomType)
                             .Include(r => r.Bookings)
                             .Where(room => room.HotelId == hotelId)
                             .ToListAsync(cancellationToken)
                             .ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException("The operation to get rooms by hotel Id was cancelled.", e, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"An error occurred while retrieving rooms for hotel Id {hotelId}", exception);
            }
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <returns>A list of rooms</returns>
        public async Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return await _azureSqlHbsDbContext.Rooms
                            .Include(r => r.Hotel)
                            .Include(r => r.RoomType)
                            .Include(r => r.Bookings)
                                .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException("The operation to get all rooms was cancelled.", e, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred while retrieving all rooms", exception);
            }
        }

        /// <summary>
        /// Removes all Rooms
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task RemoveAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                _azureSqlHbsDbContext.Rooms.RemoveRange(_azureSqlHbsDbContext.Rooms);

                await _azureSqlHbsDbContext.SaveChangesAsync();
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException("The operation to remove all rooms was cancelled.", e, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while removing all rooms", e);
            }
        }
    }
}
