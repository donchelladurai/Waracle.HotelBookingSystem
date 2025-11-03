using NUnit.Framework;
using Waracle.HotelBookingSystem.Application.Queries;

namespace Waracle.HotelBookingSystem.Application.Tests.Queries
{
    [TestFixture]
    public class GetHotelsByNameQueryTests
    {
        [Test]
        public void GetHotelsByNameQuery_SetsPropertiesCorrectly()
        {
            var name = "Test Hotel";
            var query = new GetHotelsByNameQuery(name);

            Assert.That(query.Name, Is.EqualTo(name));
        }
    }
}