using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Queries
{
    public class GetAvailableRoomsQuery : IRequest<IEnumerable<RoomDto>>
    {
        public GetAvailableRoomsQuery(DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfGuests = numberOfGuests;
        }

        public DateTime CheckInDate { get; protected set; }
        public DateTime CheckOutDate { get; protected set; }
        public int NumberOfGuests { get; protected set; }
    }
}
