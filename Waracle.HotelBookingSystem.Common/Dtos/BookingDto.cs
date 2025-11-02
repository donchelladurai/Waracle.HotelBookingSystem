using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Common.Dtos
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BookingReference { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
