using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Commands
{
    public record BookRoomCommand : IRequest<Booking>
    {
        public BookRoomCommand(int hotelId, Room room, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            HotelId = hotelId;
            Room = room;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfGuests = numberOfGuests;
        }

        public int HotelId { get; protected set; }
        public Room Room { get; protected set; }
        public DateTime CheckInDate { get; protected set; }
        public DateTime CheckOutDate { get; protected set; }
        public int NumberOfGuests { get; protected set; }

        public bool IsCheckoutDateAfterCheckInDate()
        {
            return CheckOutDate > CheckInDate;
        }
    }
}
