using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Common
{
    public record BookRoomCommandResult
    {
        public BookRoomCommandResult(bool isSuccessful, bool isRoomUnavailable, string bookingReference)
        {
            IsSuccessful = isSuccessful;
            BookingReference = bookingReference;
            IsRoomUnavailable = isRoomUnavailable;
        }

        public bool IsSuccessful { get; protected set; }
        public bool IsRoomUnavailable { get; protected set; }
        public string BookingReference { get; protected set; }
    }
}
