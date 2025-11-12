namespace Waracle.HotelBookingSystem.Domain.Entities
{
    public class Hotel : SqlEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
