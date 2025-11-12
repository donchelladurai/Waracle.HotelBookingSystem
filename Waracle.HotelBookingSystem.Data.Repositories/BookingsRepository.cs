using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Data.Repositories
{
    public class BookingsRepository : GenericSqlRepository<Booking>, IBookingsRepository
    {
        /// <summary>
        /// A repository for CRUD operations related to bookings
        /// </summary>
        /// <param name="azureSqlHbsDbContext">The Azure SQL DB Context</param>
        public BookingsRepository(AzureSqlHbsDbContext azureSqlHbsDbContext) : base(azureSqlHbsDbContext)
        {
        }

        ///// <summary>
        ///// A list of all bookings
        ///// </summary>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        ArgumentNullException.ThrowIfNull(cancellationToken);

        //        cancellationToken.ThrowIfCancellationRequested();

        //        return await _azureSqlHbsDbContext.Bookings
        //            .Include(b => b.Room)
        //                .ThenInclude(r => r.RoomType)
        //            .Include(b => b.Room)
        //                .ThenInclude(r => r.Hotel)
        //        .ToListAsync(cancellationToken).ConfigureAwait(false);
        //    }
        //    catch (OperationCanceledException e)
        //    {
        //        throw new OperationCanceledException("The operation to retrieve all bookings was cancelled.", e, cancellationToken);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("An error occurred while retrieving all bookings.", exception);
        //    }
        //}

        ///// <summary>
        ///// Find a booking based on booking reference
        ///// </summary>
        ///// <param name="bookingReference">The booking reference</param>
        ///// <returns>A booking that corresponds to the given booking reference</returns>
        //public async Task<Booking?> FindByReferenceAsync(string bookingReference, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        ArgumentNullException.ThrowIfNullOrEmpty(bookingReference);
        //        ArgumentNullException.ThrowIfNull(cancellationToken);

        //        cancellationToken.ThrowIfCancellationRequested();

        //        return await _azureSqlHbsDbContext.Bookings
        //                    .Include(b => b.Room)
        //                       .ThenInclude(r => r.RoomType)
        //                    .Include(b => b.Room)
        //                        .ThenInclude(r => r.Hotel)
        //                    .FirstOrDefaultAsync(booking => booking.Reference == bookingReference, cancellationToken).ConfigureAwait(false);
        //    }
        //    catch (OperationCanceledException e)
        //    {
        //        throw new OperationCanceledException("The operation to find a booking by reference was cancelled.", e, cancellationToken);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("An error occurred while finding a booking by reference.", exception);
        //    }
        //}

        ///// <summary>
        ///// Create a booking
        ///// </summary>
        ///// <param name="booking">The booking</param>
        //public async Task CreateAsync(Booking booking, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        ArgumentNullException.ThrowIfNull(booking);
        //        ArgumentNullException.ThrowIfNull(cancellationToken);
                
        //        cancellationToken.ThrowIfCancellationRequested();

        //        if (booking.IsValid())
        //        {
        //            await _azureSqlHbsDbContext.Bookings.AddAsync(booking, cancellationToken).ConfigureAwait(false);
        //            await _azureSqlHbsDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //        }
        //    }
        //    catch (OperationCanceledException e)
        //    {
        //        throw new OperationCanceledException("The operation to create a new booking was cancelled.", e, cancellationToken);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("An error occurred while creating a new booking.", exception);
        //    }
        //}

        ///// <summary>
        ///// Removes all Bookings
        ///// </summary>
        ///// <param name="cancellationToken"></param>
        //public async Task RemoveAllAsync(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        ArgumentNullException.ThrowIfNull(cancellationToken);

        //        cancellationToken.ThrowIfCancellationRequested();

        //        _azureSqlHbsDbContext.Bookings.RemoveRange(_azureSqlHbsDbContext.Bookings);

        //        await _azureSqlHbsDbContext.SaveChangesAsync();
        //    }
        //    catch (OperationCanceledException e)
        //    {
        //        throw new OperationCanceledException("The operation to remove all bookings was cancelled.", e, cancellationToken);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("An error occurred while removing all bookings", e);
        //    }
        //}
    }
}
