namespace Waracle.HotelBookingSystem.Common.Enums
{
    public enum Errors
    {
        None,
        CommandIsNull,
        CancellationTokenNotProvided,
        RoomNotAvailable,
        InvalidNumberOfGuests,
        CheckOutDateBeforeCheckInDate
    }
}
