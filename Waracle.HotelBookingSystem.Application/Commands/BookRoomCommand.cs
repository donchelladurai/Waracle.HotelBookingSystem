using MediatR;
using Waracle.HotelBookingSystem.Common;

namespace Waracle.HotelBookingSystem.Application.Commands
{
    public sealed record BookRoomCommand : IRequest<Result<string>>
    {
        public BookRoomCommand(int hotelId, int roomId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            HotelId = hotelId;
            RoomId = roomId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfGuests = numberOfGuests;
        }

        public int HotelId { get; }
        public int RoomId { get; }
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }
        public int NumberOfGuests { get; }

        public bool IsCheckoutDateAfterCheckInDate()
        {
            return CheckOutDate > CheckInDate;
        }
    }
}
