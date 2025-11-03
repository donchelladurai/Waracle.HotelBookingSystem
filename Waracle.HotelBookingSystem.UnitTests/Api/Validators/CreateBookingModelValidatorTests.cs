using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

namespace Waracle.HotelBookingSystem.UnitTests.Api.Validators
{
    [TestFixture]
    public class CreateBookingModelValidatorTests
    {
        private CreateBookingModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new CreateBookingModelValidator();
        }

        [Test]
        public async Task Validate_ValidModel_ReturnsValid()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 1,
                RoomId = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfGuests = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public async Task Validate_InvalidHotelId_ReturnsInvalid()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 0,
                RoomId = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfGuests = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Hotel Id is invalid"));
        }

        [Test]
        public async Task Validate_InvalidRoomId_ReturnsInvalid()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 1,
                RoomId = 0,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfGuests = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Room Id is invalid"));
        }

        [Test]
        public async Task Validate_InvalidNumberOfGuests_ReturnsInvalid()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 1,
                RoomId = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfGuests = 0
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Number of guests must be at least 1"));
        }

        [Test]
        public async Task Validate_SameCheckInCheckOut_ReturnsInvalid()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1);
            var model = new CreateBookingModel
            {
                HotelId = 1,
                RoomId = 1,
                CheckInDate = date,
                CheckOutDate = date,
                NumberOfGuests = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage,
                Is.EqualTo("Check-in date and check-out date cannot be the same."));
        }

        [Test]
        public async Task Validate_CheckOutBeforeCheckIn_ReturnsInvalid()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 1,
                RoomId = 1,
                CheckInDate = DateTime.Now.AddDays(2),
                CheckOutDate = DateTime.Now.AddDays(1),
                NumberOfGuests = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage,
                Is.EqualTo("Check-out date must be later than the Check-in date"));
        }
    }
}
