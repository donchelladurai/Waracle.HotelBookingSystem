using System.ComponentModel.DataAnnotations;

namespace Waracle.HotelBookingSystem.Web.Api.Models
{
    public class CreateBookingModel
    {
        [Required]
        public int HotelId { get; set; }
        [Required]
        public int RoomId { get; set; }

        /// <summary>
        /// The check-in date for the booking in ISO Format YYYY-MM-DD
        /// </summary>
        [Required]
        public DateTime CheckInDate { get; set; }
        /// <summary>
        /// The check-out date for the booking in ISO Format YYYY-MM-DD
        /// </summary>
        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }
    }
}
