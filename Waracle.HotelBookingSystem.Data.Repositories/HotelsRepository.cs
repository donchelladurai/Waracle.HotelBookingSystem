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
    /// <remarks>This repository interacts with the <see cref="HotelsDbContext"/> to perform operations
    /// related to hotels, rooms, and bookings. It includes methods for retrieving hotel information, checking room
    /// availability, and managing bookings. The repository also provides functionality to seed the database with test
    /// data and remove all transactional data.</remarks>
    public class HotelRepository : IHotelsRepository
    {
        private readonly HotelsDbContext _hotelsDbcontext;
        private readonly List<string> _hotelNamesSeedData = [
            "California",
            "Novotel",
            "Premier Inn",
            "Holiday Inn",
            "Budget Inn",
            "Savoy"];
        private readonly Random _random = new();
        private static long _counter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelRepository"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The <see cref="HotelsDbContext"/> used to access hotel data. Cannot be null.</param>
        public HotelRepository(HotelsDbContext context)
        {
            _hotelsDbcontext = context;
        }

        /// <summary>
        /// Asynchronously retrieves a hotel by its name, including its associated rooms.
        /// </summary>
        /// <param name="name">The name of the hotel to retrieve. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the hotel with the specified
        /// name, or <see langword="null"/> if no hotel with that name exists.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving the hotel.</exception>
        public async Task<IEnumerable<Hotel>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                var hotels = await _hotelsDbcontext.Hotels
                    .AsNoTracking()
                    .Include(h => h.Rooms)
                    .Where(h => h.Name.Contains(name))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return hotels;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the hotel by name", e);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of available rooms for the specified date range and number of guests.
        /// </summary>
        /// <param name="checkInDate">The desired check-in date for the room booking.</param>
        /// <param name="checkOutDate">The desired check-out date for the room booking.</param>
        /// <param name="numberOfGuests">The number of guests that the room must accommodate.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of available rooms that
        /// meet the specified criteria.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving available rooms.</exception>
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int numberOfGuests, CancellationToken cancellationToken)
        {
            try
            {
                var rooms = await _hotelsDbcontext.Rooms
                            .AsNoTracking()
                            .Where(r => r.RoomType.Capacity >= numberOfGuests)
                            .Where(r => !_hotelsDbcontext.Bookings.Any(b => b.RoomId == r.Id &&
                                (b.CheckInDate < checkOutDate && b.CheckOutDate > checkInDate)))
                            .ToListAsync(cancellationToken);

                return rooms;
            }

            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving available rooms", e);
            }
        }

        /// <summary>
        /// Retrieves a list of available rooms in a specified hotel for the given date range and number of guests.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to search for available rooms.</param>
        /// <param name="checkInDate">The start date of the desired booking period.</param>
        /// <param name="checkOutDate">The end date of the desired booking period.</param>
        /// <param name="numberOfGuests">The number of guests that the room must accommodate.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of available rooms that meet
        /// the specified criteria.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving available rooms.</exception>
        public async Task<IEnumerable<Room>> GetAvailableRoomsInHotelAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests, CancellationToken cancellationToken)
        {
            try
            {
                var rooms = await _hotelsDbcontext.Rooms
                            .AsNoTracking() 
                            .Where(r => r.HotelId == hotelId && r.RoomType.Capacity >= numberOfGuests)
                            .Where(r => !_hotelsDbcontext.Bookings.Any(b => b.RoomId == r.Id &&
                                (b.CheckInDate < checkOutDate && b.CheckOutDate > checkInDate)))
                            .ToListAsync(cancellationToken);

                return rooms;
            }

            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving available rooms", e);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a booking by its reference number.
        /// </summary>
        /// <remarks>This method queries the database to find a booking that matches the provided
        /// reference number.  It includes related room and hotel information in the result.</remarks>
        /// <param name="reference">The unique reference number of the booking to retrieve. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The <see cref="Booking"/> object associated with the specified reference number, or <see langword="null"/>
        /// if no booking is found.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving the booking.</exception>
        public async Task<Booking> GetBookingByReferenceAsync(string reference, CancellationToken cancellationToken)
        {
            try
            {
                Booking? booking = await _hotelsDbcontext.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(b => b.Reference == reference);

                return booking;
            }
            catch(Exception e)
            {
                throw new Exception("An error occurred while retrieving a booking", e);
            }
        }

        /// <summary>
        /// Adds a new booking to the database if the room is available for the specified dates.
        /// </summary>
        /// <remarks>This method checks the availability of the room before adding the booking. If the
        /// room is available, it generates a booking reference and saves the booking to the database. If the room is
        /// not available, an exception is thrown.</remarks>
        /// <param name="booking">The booking details to be added, including room ID, check-in, and check-out dates.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown if an error occurs while adding the booking.</exception>
        public async Task AddBookingAsync(Booking booking, CancellationToken cancellationToken)
        {
            try
            {
                if (await IsRoomAvailable(booking.RoomId, booking.CheckInDate, booking.CheckOutDate, cancellationToken))
                {
                    booking.Reference = GenerateBookingReference();
                    _hotelsDbcontext.Bookings.Add(booking);

                    await _hotelsDbcontext.SaveChangesAsync(cancellationToken);
                }

                else throw new Exception("Room not available");
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while adding the booking", e);
            }
        }

        /// <summary>
        /// Retrieves a hotel by its unique identifier, including its associated rooms.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to retrieve. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests, which can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Hotel"/> object
        /// with the specified identifier, or throws an exception if not found.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving the hotel by Id.</exception>
        public Task<Hotel> GetHotelById(int hotelId, CancellationToken cancellationToken)
        {
            try
            {
                return _hotelsDbcontext.Hotels.Include(h => h.Rooms).AsQueryable().AsNoTracking().FirstAsync(r => r.Id == hotelId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the hotel by Id", e);
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
                if (!_hotelsDbcontext.Hotels.Any())
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

                        _hotelsDbcontext.Hotels.Add(hotel);
                    }

                    await _hotelsDbcontext.SaveChangesAsync();
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
                _hotelsDbcontext.Bookings.RemoveRange(_hotelsDbcontext.Bookings);
                _hotelsDbcontext.Rooms.RemoveRange(_hotelsDbcontext.Rooms);
                _hotelsDbcontext.Hotels.RemoveRange(_hotelsDbcontext.Hotels);

                await _hotelsDbcontext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while removing transactional data", e);
            }
        }

        /// <summary>
        /// Determines whether a room is available for booking within the specified date range.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to check for availability.</param>
        /// <param name="checkIn">The desired check-in date and time.</param>
        /// <param name="checkOut">The desired check-out date and time.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns><see langword="true"/> if the room is available for the entire specified date range; otherwise, <see
        /// langword="false"/>.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while checking room availability.</exception>
        private async Task<bool> IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
        {
            try
            {
                return !await _hotelsDbcontext.Bookings.AnyAsync(b => b.RoomId == roomId && (b.CheckInDate < checkOut && b.CheckOutDate > checkIn));
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while checking room availability", e);
            }
        }

        /// <summary>
        /// Generates a unique hotel booking reference.
        /// </summary>
        /// <returns>A unique booking reference string.</returns>
        private string GenerateBookingReference()
        {
            DateTime now = DateTime.UtcNow;
            string datePart = now.ToString("yyyyMMdd-HHmmss");

            long uniqueId = Interlocked.Increment(ref _counter);
            string idPart = uniqueId.ToString("D3");

            return $"{datePart}-{idPart}";
        }
    }
}
