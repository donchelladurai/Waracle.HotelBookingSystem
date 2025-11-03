using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.AutomationTests.Helpers;
using Waracle.HotelBookingSystem.Application;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Web.Api.Models;

namespace Waracle.HotelBookingSystem.AutomationTests
{
    [TestFixture]
    public class BookingsControllerTests : ApiTestsBase
    {
        [Test]
        public async Task GetAllBookings_ShouldReturnBookings_WhenExists()
        {
            // Act
            var response = await HttpClient.ExecuteGetAsync<List<BookingDto>>("/api/bookings");

            // Assert
            Assert.That((int)response.StatusCode == 200);

            var bookings = response.Data;
            Assert.IsNotNull(bookings);
        }

        [Test]
        public async Task FindBookingByReference_ShouldReturn200_WhenValidReference()
        {
            // Arrange: First create a booking to get a reference
            var booking = GetSampleBooking().Result;

            // Act
            var response = await HttpClient.ExecuteGetAsync<BookingDto>($"/api/bookings/reference?bookingReference={booking.BookingReference}");

            // Assert
            Assert.That((int)response.StatusCode == 200);

            Assert.IsNotNull(response.Data);
            Assert.That(booking.BookingReference.Equals(response.Data.BookingReference));
        }

        [Test]
        public async Task FindBookingByReference_ShouldReturn404_WhenInvalidReference()
        {
            //arrange
            string invalidRef = "InvalidRef123";

            //Act
            var response = await HttpClient.ExecuteGetAsync<string>($"/api/bookings/{invalidRef}");

            //Assert
            Assert.That((int)response.StatusCode == 404);
        }

        [Test]
        public async Task CreateBooking_ShouldReturnMessage_WhenRoomUnavailable()
        {
            // Arrange
            var booking = await GetSampleBooking();

            var model = new CreateBookingModel
            {
                HotelId = booking.HotelId,
                RoomId = booking.RoomId,
                CheckInDate = DateTime.Now.AddDays(10),
                CheckOutDate = DateTime.Now.AddDays(12),
                NumberOfGuests = 2
            };

            // Act
            var response = await HttpClient.ExecutePostAsync<string>("/api/bookings", model);

            // Assert
            Assert.That((int)response.StatusCode == 200, $"The selected room is not available for {booking.NumberOfGuests} occupants between {booking.CheckInDate} and {booking.CheckOutDate}");
        }

        [Test]
        public async Task CreateBooking_ShouldReturn400_WhenInvalidModel()
        {
            var invalidModel = new CreateBookingModel
            {
                HotelId = -100,
                RoomId = -50,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(-1),
                NumberOfGuests = 500
            };

            var response = await HttpClient.ExecutePostAsync<string>("/api/bookings", invalidModel);

            Assert.That((int)response.StatusCode == 400);
        }

        [Test]
        public async Task CreateBooking_ShouldReturn200_WhenValid()
        {
            // Arrange
            var availableRooms = await GetAvailableRooms(DateTime.Now, DateTime.Now.AddYears(5));

            // Act
            var model = new CreateBookingModel
            {
                HotelId = availableRooms.First().HotelId,
                RoomId = availableRooms.First().RoomId,
                CheckInDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                CheckOutDate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")),
                NumberOfGuests = availableRooms.First().Capacity
            };

            var response = await HttpClient.ExecutePostAsync<string>("/api/bookings", model);

            // Assert
            Assert.That((int)response.StatusCode == 200);
        }

        private async Task<BookingDto> GetSampleBooking()
        {
            var response = await HttpClient.ExecuteGetAsync<string>("/api/bookings");
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null && response.Content.Count() > 0)
            {
                var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BookingDto>>(response.Content);

                return bookings.FirstOrDefault();
            }

            return null;
        }

        private async Task<IEnumerable<RoomDto>> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var response = await HttpClient.ExecuteGetAsync<string>($"/api/rooms?checkInDate={checkIn.ToShortDateString()}&checkOutDate={checkOut.ToShortDateString()}&numberOfOccupants=1");
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null && response.Content.Count() > 0)
            {
                var rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoomDto>>(response.Content);

                return rooms;
            }

            return null;
        }
    }
}
