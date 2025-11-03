using Waracle.HotelBookingSystem.AutomationTests.Helpers;
using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.AutomationTests
{
    public class ApiTestsBase
    {
        private const string BaseUrl =
            "https://waracle-hotelbookingsystem-webapi-fjgpd5d5hwdrbxbt.canadacentral-01.azurewebsites.net";

        protected readonly RestClientHelper RestHttpClient = new RestClientHelper(BaseUrl);

        protected async Task<BookingDto> GetSampleBooking()
        {
            var response = await RestHttpClient.ExecuteGetAsync<string>("/api/bookings");
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null &&
                response.Content.Count() > 0)
            {
                var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BookingDto>>(response.Content);

                return bookings.FirstOrDefault();
            }

            return null;
        }

        protected async Task<IEnumerable<RoomDto>> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var response = await RestHttpClient.ExecuteGetAsync<string>(
                $"/api/rooms?checkInDate={checkIn.ToShortDateString()}&checkOutDate={checkOut.ToShortDateString()}&numberOfOccupants=1");
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null &&
                response.Content.Count() > 0)
            {
                var rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoomDto>>(response.Content);

                return rooms;
            }

            return null;
        }
    }
}
