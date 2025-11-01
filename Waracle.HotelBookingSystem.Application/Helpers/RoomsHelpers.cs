using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Helpers
{
    public static class RoomsHelpers
    {
        public static IEnumerable<Room> GetAvailableRooms(this IEnumerable<Room> rooms, int numberOfGuests, DateTime checkInDate, DateTime checkOutDate)
        {
            return rooms.Where(r => r.RoomType.Capacity >= numberOfGuests &&
                                    !r.Bookings.Any(b => b.RoomId == r.Id &&
                                                         (b.CheckInDate < checkOutDate && b.CheckOutDate > checkInDate)));
        }
    }
}
