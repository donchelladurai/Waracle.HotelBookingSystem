using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Common
{
    public record BookRoomCommandResult
    {
        public BookRoomCommandResult(bool isSucessful, string bookingReference)
        {
            IsSucessful = isSucessful;
            BookingReference = bookingReference;
        }

        public bool IsSucessful { get; protected set; }
        public string BookingReference { get; protected set; }
    }
}
