namespace Waracle.HotelBookingSystem.Common
{
    public sealed record BookRoomCommandResult
    {
        public BookRoomCommandResult(bool isSuccessful, bool isRoomUnavailable, string bookingReference)
        {
            IsSuccessful = isSuccessful;
            BookingReference = bookingReference;
            IsRoomUnavailable = isRoomUnavailable;
        }

        public bool IsSuccessful { get; }
        public bool IsRoomUnavailable { get; }
        public string BookingReference { get; }
    }
}
