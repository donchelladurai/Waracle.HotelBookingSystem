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
        Task SeedTestDataAsync();
        Task RemoveAllTransactionalDataAsync();

        Task<Hotel> GetByNameAsync(string name);
        Task<List<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests);
        Task<Booking> GetBookingByReferenceAsync(string reference);
        Task AddBookingAsync(Booking booking);
    }
}
