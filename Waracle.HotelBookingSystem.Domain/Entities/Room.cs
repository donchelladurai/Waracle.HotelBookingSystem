namespace Waracle.HotelBookingSystem.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public int RoomTypeId { get; set; } 
        public RoomType RoomType { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
