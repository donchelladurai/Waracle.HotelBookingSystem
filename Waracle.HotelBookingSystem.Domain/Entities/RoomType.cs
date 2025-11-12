namespace Waracle.HotelBookingSystem.Domain.Entities
{
    public class RoomType : SqlEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}
