namespace Waracle.HotelBookingSystem.Web.Api.Models
{
    public class CreateBookingModel
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
