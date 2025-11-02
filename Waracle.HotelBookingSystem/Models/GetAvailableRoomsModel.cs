namespace Waracle.HotelBookingSystem.Web.Api.Models
{
    public class GetAvailableRoomsModel
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfOccupants { get; set; }
    }
}
