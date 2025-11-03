using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Web.Api.Controllers;

namespace Waracle.HotelBookingSystem.UnitTests.Api.Controllers
{
    [TestFixture]
    public class RoomsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<RoomsController>> _loggerMock;
        private RoomsController _systemUnderTest;

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<RoomsController>>();
            _systemUnderTest = new RoomsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAvailableRoomsAsync_ValidModel_ReturnsOkWithRooms()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(2);
            var occupants = 2;
            var rooms = new List<RoomDto> { new RoomDto { RoomId = 1 } };
            _mediatorMock.Setup(m =>
                    m.Send(
                        It.Is<GetAvailableRoomsQuery>(q =>
                            q.CheckInDate == checkIn && q.CheckOutDate == checkOut && q.NumberOfGuests == occupants),
                        default))
                .ReturnsAsync(rooms);

            // Act
            var result = await _systemUnderTest.GetAvailableRoomsAsync(checkIn, checkOut, occupants, default);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(rooms));
        }

        [Test]
        public async Task GetAvailableRoomsAsync_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(2);
            var checkOut = DateTime.Now.AddDays(1);
            var occupants = 0;

            // Act
            var result = await _systemUnderTest.GetAvailableRoomsAsync(checkIn, checkOut, occupants, default);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAvailableRoomsAsync_NoRoomsFound_ReturnsNotFound()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(2);
            var occupants = 2;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableRoomsQuery>(), default))
                .ReturnsAsync(new List<RoomDto>());

            // Act
            var result = await _systemUnderTest.GetAvailableRoomsAsync(checkIn, checkOut, occupants, default);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAvailableRoomsAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(2);
            var occupants = 2;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableRoomsQuery>(), default))
                .ThrowsAsync(new Exception("Boingggg!"));

            // Act
            var result = await _systemUnderTest.GetAvailableRoomsAsync(checkIn, checkOut, occupants, default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetAvailableRoomsAsync_CancellationRequested_Returns499()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(2);
            var occupants = 2;
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _systemUnderTest.GetAvailableRoomsAsync(checkIn, checkOut, occupants, cts.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(499));
        }
    }
}