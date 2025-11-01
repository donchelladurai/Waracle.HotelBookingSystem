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

        public HotelRepository(HotelsDbContext context)
        {
            _hotelsDbcontext = context;
        }

        public async Task<Hotel> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                Hotel? hotel = await _hotelsDbcontext.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Name == name);

                return hotel;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the hotel by name", e);
            }
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests, CancellationToken cancellationToken)
        {
            try
            {
                var rooms = await _hotelsDbcontext.Rooms
                .Where(r => r.HotelId == hotelId && r.RoomType.Capacity >= guests)
                .Where(r => !_hotelsDbcontext.Bookings.Any(b => b.RoomId == r.Id &&
                    (b.CheckInDate < checkOut && b.CheckOutDate > checkIn)))
                .ToListAsync();

                return rooms;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving available rooms", e);
            }
        }

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

        public Task<Room> GetRoomById(int roomId, CancellationToken cancellationToken)
        {
            try
            {
                return _hotelsDbcontext.Rooms.AsQueryable().AsNoTracking().FirstAsync(r => r.Id == roomId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the room by ID", e);
            }
        }

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
