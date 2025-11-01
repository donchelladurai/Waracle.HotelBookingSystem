using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public interface IHotelsRepository
    {
        Task SeedTestDataAsync(CancellationToken cancellationToken);
        Task RemoveAllTransactionalDataAsync(CancellationToken cancellationToken);

        Task<Hotel> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<List<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests, CancellationToken cancellationToken);
        Task<Booking> GetBookingByReferenceAsync(string reference, CancellationToken cancellationToken);
        Task AddBookingAsync(Booking booking, CancellationToken cancellationToken);
        Task<Room> GetRoomById(int roomId, CancellationToken cancellationToken);
    }
}
