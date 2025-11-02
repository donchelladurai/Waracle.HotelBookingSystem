namespace Waracle.HotelBookingSystem.Web.Api.Models
{
    public class CreateBookingModel
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }

        /// <summary>
        /// The check-in date for the booking in DD/MM/YYYY format
        /// </summary>
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
