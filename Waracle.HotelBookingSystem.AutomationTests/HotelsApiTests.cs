using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.AutomationTests.Helpers;
using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.AutomationTests
{
    [TestFixture]
    public class HotelsApiTests : ApiTestsBase
    {
        [Test]
        public async Task GetHotelsByName_ShouldReturnHotels_WhenNameMatches()
        {
            string travelodge = "Travelodge";
            string inn = "inn";

            // Act
            var travelodgeResponse = await RestHttpClient.ExecuteGetAsync<List<HotelDto>>($"/api/hotels/{travelodge}");
            var innResponse = await RestHttpClient.ExecuteGetAsync<List<HotelDto>>($"/api/hotels/{inn}");

            // Assert
            Assert.That((int)travelodgeResponse.StatusCode == 200);
            Assert.That((int)innResponse.StatusCode == 200);

            Assert.That(travelodgeResponse.Data != null);
            Assert.That(innResponse.Data != null);

            Assert.That(travelodgeResponse.Data.Count == 1);
            Assert.That(innResponse.Data.Count == 3);

            Assert.IsTrue(travelodgeResponse.Data[0].Name.Contains(travelodge, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(innResponse.Data[0].Name.Contains(inn, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public async Task GetHotelsByName_ShouldReturn404_WhenNoMatches()
        {
            // Arrange
            var invalidHotelName = "NonExistentHotel";

            // Act
            var response = await RestHttpClient.ExecuteGetAsync<string>($"/api/hotels/{invalidHotelName}");

            // Assert
            Assert.That((int)response.StatusCode == 404);
        }
    }
}
