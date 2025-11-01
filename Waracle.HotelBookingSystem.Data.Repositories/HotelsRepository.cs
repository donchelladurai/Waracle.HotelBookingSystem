using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Data.Repositories
{
    /// <summary>
    /// Provides methods for managing hotel data, including retrieving hotels and rooms, managing bookings, and seeding
    /// test data.
    /// </summary>
    /// <remarks>This repository interacts with the <see cref="AzureSqlHbsDbContext"/> to perform operations
    /// related to hotels, rooms, and bookings. It includes methods for retrieving hotel information, checking room
    /// availability, and managing bookings. The repository also provides functionality to seed the database with test
    /// data and remove all transactional data.</remarks>
    public class HotelRepository : IHotelsRepository
    {
        private readonly AzureSqlHbsDbContext _azureSqlHbsDbContext;
        private readonly List<string> _hotelNamesSeedData = [
            "California",
            "Novotel",
            "Premier Inn",
            "Holiday Inn",
            "Budget Inn",
            "Savoy"];

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelRepository"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The <see cref="AzureSqlHbsDbContext"/> used to access hotel data. Cannot be null.</param>
        public HotelRepository(AzureSqlHbsDbContext context)
        {
            _azureSqlHbsDbContext = context;
        }

        /// <summary>
        /// Gets a hotel by its hotel id
        /// </summary>
        /// <param name="hotelId">The hotel Id</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A hotel that corresponds to the given id</returns>
        public async Task<Hotel?> GetByIdAsync(int hotelId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await _azureSqlHbsDbContext.Hotels
                    .AsNoTracking()
                    .Include(h => h.Rooms)
                    .FirstOrDefaultAsync(h => h.Id == hotelId, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException("The operation to get hotel by Id was cancelled.", e, cancellationToken);  
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the hotel by Id", e);
            }
        }

        /// <summary>
        /// Get all hotels where the hotel name contains the given name
        /// </summary>
        /// <param name="name">The name to search by</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A hotel that contains the given name</returns>
        public async Task<IEnumerable<Hotel>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hotels = await _azureSqlHbsDbContext.Hotels
                    .AsNoTracking()
                    .Include(h => h.Rooms)
                    .Where(h => h.Name.Contains(name))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return hotels;
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException("The operation to get hotel by name was cancelled.", e, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the hotel by name", e);
            }
        }

        /// <summary>
        /// Seeds the database with test data for hotels and their rooms if no hotels currently exist.
        /// </summary>
        /// <remarks>This method adds a predefined set of hotels and associated rooms to the database.  It
        /// checks if the database is empty before seeding to prevent duplicate entries.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown if an error occurs during the seeding process.</exception>
        public async Task SeedTestDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_azureSqlHbsDbContext.Hotels.Any())
                {
                    foreach (var hotelName in _hotelNamesSeedData)
                    {
                        var hotel = new Hotel { Name = hotelName };

                        hotel.Rooms.Add(new Room { RoomTypeId = 1 });
                        hotel.Rooms.Add(new Room { RoomTypeId = 1 });
                        hotel.Rooms.Add(new Room { RoomTypeId = 2 });
                        hotel.Rooms.Add(new Room { RoomTypeId = 2 });
                        hotel.Rooms.Add(new Room { RoomTypeId = 3 });
                        hotel.Rooms.Add(new Room { RoomTypeId = 3 });

                        _azureSqlHbsDbContext.Hotels.Add(hotel);
                    }

                    await _azureSqlHbsDbContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while seeding test data", e);
            }
        }

        /// <summary>
        /// Asynchronously removes all transactional data from the database, including bookings, rooms, and hotels.
        /// </summary>
        /// <remarks>This method deletes all entries in the Bookings, Rooms, and Hotels tables. Use with
        /// caution as this operation is irreversible.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown if an error occurs during the removal of transactional data.</exception>
        public async Task RemoveAllTransactionalDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _azureSqlHbsDbContext.Bookings.RemoveRange(_azureSqlHbsDbContext.Bookings);
                _azureSqlHbsDbContext.Rooms.RemoveRange(_azureSqlHbsDbContext.Rooms);
                _azureSqlHbsDbContext.Hotels.RemoveRange(_azureSqlHbsDbContext.Hotels);

                await _azureSqlHbsDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while removing transactional data", e);
            }
        }
    }
}
