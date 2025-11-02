using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Common.Dtos
{
    public class BookingDto
    {
        public int HotelId { get; set; }

        public int RoomId { get; set; }
        public string BookingReference { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
