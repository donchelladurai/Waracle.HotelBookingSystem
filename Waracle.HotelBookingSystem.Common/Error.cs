using Waracle.HotelBookingSystem.Common.Enums;

namespace Waracle.HotelBookingSystem.Common
{
    public sealed record Error(Errors ErrorType, string Description)
    {
        public static readonly Error None = new(Errors.None, string.Empty);
        public static readonly Error CommandIsNull = new(Errors.CommandIsNull, "The command provided to the CQRS handler is null");
        public static readonly Error CancellationTokenNotProvided = new(Errors.CancellationTokenNotProvided, "No cancellation token provided");
        public static readonly Error RoomNotAvailable = new(Errors.RoomNotAvailable, "The room requested for booking is not available for the given dates");
        public static readonly Error InvalidNumberOfGuests = new(Errors.InvalidNumberOfGuests, "The number of guests is invalid");
        public static readonly Error CheckOutDateBeforeCheckInDate = new(Errors.CheckOutDateBeforeCheckInDate, "The check out date provided is earlier than the check in date");
    }
}
