using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

namespace Waracle.HotelBookingSystem.UnitTests.Api.Validators
{
    [TestFixture]
    public class GetAvailableRoomsModelValidatorTests
    {
        private GetAvailableRoomsModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetAvailableRoomsModelValidator();
        }

        [Test]
        public async Task Validate_ValidModel_ReturnsValid()
        {
            // Arrange
            var model = new GetAvailableRoomsModel
            {
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfOccupants = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public async Task Validate_SameCheckInCheckOut_ReturnsInvalid()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1);
            var model = new GetAvailableRoomsModel
            {
                CheckInDate = date,
                CheckOutDate = date,
                NumberOfOccupants = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Check-in date and check-out date cannot be the same."));
        }

        [Test]
        public async Task Validate_CheckInAfterCheckOut_ReturnsInvalid()
        {
            // Arrange
            var model = new GetAvailableRoomsModel
            {
                CheckInDate = DateTime.Now.AddDays(2),
                CheckOutDate = DateTime.Now.AddDays(1),
                NumberOfOccupants = 2
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Check-in date must be earlier than check-out date."));
        }

        [Test]
        public async Task Validate_InvalidNumberOfOccupants_ReturnsInvalid()
        {
            // Arrange
            var model = new GetAvailableRoomsModel
            {
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfOccupants = 0
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("Number of occupants must be greater than zero."));
        }
    }
}
