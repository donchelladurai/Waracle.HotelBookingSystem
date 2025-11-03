using Waracle.HotelBookingSystem.Application.Queries;

namespace Waracle.HotelBookingSystem.Application.Tests.Queries
{
    [TestFixture]
    public class GetBookingByReferenceQueryTests
    {
        [Test]
        public void GetBookingByReferenceQuery_SetsPropertiesCorrectly()
        {
            var reference = "REF-123";
            var query = new GetBookingByReferenceQuery(reference);

            Assert.That(query.BookingReference, Is.EqualTo(reference));
        }
    }
}