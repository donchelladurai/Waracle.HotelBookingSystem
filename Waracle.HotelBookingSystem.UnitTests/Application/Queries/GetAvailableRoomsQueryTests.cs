using Waracle.HotelBookingSystem.Application.Queries;

namespace Waracle.HotelBookingSystem.Application.Tests.Queries
{
    [TestFixture]
    public class GetAvailableRoomsQueryTests
    {
        [Test]
        public void GetAvailableRoomsQuery_SetsPropertiesCorrectly()
        {
            var checkIn = DateTime.Now;
            var checkOut = checkIn.AddDays(1);
            var guests = 2;
            var query = new GetAvailableRoomsQuery(checkIn, checkOut, guests);

            Assert.That(query.CheckInDate, Is.EqualTo(checkIn));
            Assert.That(query.CheckOutDate, Is.EqualTo(checkOut));
            Assert.That(query.NumberOfGuests, Is.EqualTo(guests));
        }
    }
}