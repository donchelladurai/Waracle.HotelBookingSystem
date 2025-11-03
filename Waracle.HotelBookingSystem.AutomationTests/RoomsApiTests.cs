using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.AutomationTests
{
    [TestFixture]
    public class RoomsApiTests : ApiTestsBase
    {
        [Test]
        public async Task GetAvailableRooms_ShouldReturn200_WhenAvailable()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>
            {
                { "checkInDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "checkOutDate", DateTime.Now.AddYears(10).ToString("yyyy-MM-dd") },
                { "numberOfOccupants", "1" }
            };

            // Act
            var response = await RestHttpClient.ExecuteGetAsync<List<RoomDto>>("/api/rooms", queryParams);

            // Assert
            Assert.That((int)response.StatusCode == 200);
            Assert.That(response.Data.Count > 0);
        }

        [Test]
        public async Task GetAvailableRooms_ShouldReturn400_WhenInvalidDates()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>
            {
                { "checkInDate", DateTime.Now.AddDays(3).ToString("yyyy-MM-dd") },
                { "checkOutDate", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") },
                { "numberOfOccupants", "2" }
            };

            // Act
            var response = await RestHttpClient.ExecuteGetAsync<string>("/api/rooms", queryParams);

            // Assert
            Assert.That((int)response.StatusCode == 400);
        }
    }
}
