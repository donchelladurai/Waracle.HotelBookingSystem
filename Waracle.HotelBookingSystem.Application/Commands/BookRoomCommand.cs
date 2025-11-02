using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Common;

namespace Waracle.HotelBookingSystem.Application.Commands
{
    public record BookRoomCommand : IRequest<BookRoomCommandResult>
    {
        public BookRoomCommand(int hotelId, int roomId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            HotelId = hotelId;
            RoomId = roomId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfGuests = numberOfGuests;
        }

        public int HotelId { get; protected set; }
        public int RoomId { get; protected set; }
        public DateTime CheckInDate { get; protected set; }
        public DateTime CheckOutDate { get; protected set; }
        public int NumberOfGuests { get; protected set; }

        public bool IsCheckoutDateAfterCheckInDate()
        {
            return CheckOutDate > CheckInDate;
        }
    }
}
