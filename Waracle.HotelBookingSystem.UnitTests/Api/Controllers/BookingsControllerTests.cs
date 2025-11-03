using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Web.Api.Controllers;
using Waracle.HotelBookingSystem.Web.Api.Models;

namespace Waracle.HotelBookingSystem.UnitTests.Api.Controllers
{
    internal class BookingsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<BookingsController>> _loggerMock;
        private BookingsController _systemUnderTest;

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<BookingsController>>();
            _systemUnderTest = new BookingsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllBookingsAsync_ReturnsOkWithBookings()
        {
            // Arrange
            var bookings = new List<BookingDto> { new BookingDto() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingsQuery>(), default))
                .ReturnsAsync(bookings);

            // Act
            var result = await _systemUnderTest.GetAllBookingsAsync(default);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(bookings));
        }

        [Test]
        public async Task GetAllBookingsAsync_NoBookings_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingsQuery>(), default))
                .ReturnsAsync(new List<BookingDto>());

            // Act
            var result = await _systemUnderTest.GetAllBookingsAsync(default);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllBookingsAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingsQuery>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _systemUnderTest.GetAllBookingsAsync(default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetAllBookingsAsync_CancellationRequested_Returns499()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _systemUnderTest.GetAllBookingsAsync(cts.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(499));
        }

        [Test]
        public async Task FindBookingByReferenceAsync_ValidReference_ReturnsOkWithBooking()
        {
            // Arrange
            var reference = "REF123";
            var booking = new BookingDto { BookingReference = reference };
            _mediatorMock.Setup(m => m.Send(It.Is<GetBookingByReferenceQuery>(q => q.BookingReference == reference), default))
                .ReturnsAsync(booking);

            // Act
            var result = await _systemUnderTest.FindBookingByReferenceAsync(reference, default);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(booking));
        }

        [Test]
        public async Task FindBookingByReferenceAsync_EmptyReference_ReturnsBadRequest()
        {
            // Arrange
            var reference = string.Empty;

            // Act
            var result = await _systemUnderTest.FindBookingByReferenceAsync(reference, default);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task FindBookingByReferenceAsync_NoBookingFound_ReturnsNotFound()
        {
            // Arrange
            var reference = "REF123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByReferenceQuery>(), default))
                .ReturnsAsync((BookingDto)null);

            // Act
            var result = await _systemUnderTest.FindBookingByReferenceAsync(reference, default);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task FindBookingByReferenceAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var reference = "REF123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByReferenceQuery>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _systemUnderTest.FindBookingByReferenceAsync(reference, default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task FindBookingByReferenceAsync_CancellationRequested_Returns499()
        {
            // Arrange
            var reference = "REF123";
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByReferenceQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            var result = await _systemUnderTest.FindBookingByReferenceAsync(reference, cts.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(499));
        }

        [Test]
        public async Task CreateBookingAsync_ValidModel_Successful_ReturnsOk()
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
            var commandResult = new BookRoomCommandResult(true, false, "REF123");
            _mediatorMock.Setup(m => m.Send(It.IsAny<BookRoomCommand>(), default))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, default);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new CreateBookingModel
            {
                HotelId = 0,
                RoomId = 0,
                CheckInDate = DateTime.Now.AddDays(2),
                CheckOutDate = DateTime.Now.AddDays(1),
                NumberOfGuests = 0
            };

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, default);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_RoomUnavailable_ReturnsConflict()
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
            var commandResult = new BookRoomCommandResult(false, true, string.Empty);
            _mediatorMock.Setup(m => m.Send(It.IsAny<BookRoomCommand>(), default))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(409));
        }

        [Test]
        public async Task CreateBookingAsync_Unsuccessful_ReturnsInternalServerError()
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
            var commandResult = new BookRoomCommandResult(false, false, string.Empty);
            _mediatorMock.Setup(m => m.Send(It.IsAny<BookRoomCommand>(), default))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateBookingAsync_Exception_ReturnsInternalServerError()
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
            _mediatorMock.Setup(m => m.Send(It.IsAny<BookRoomCommand>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, default);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateBookingAsync_CancellationRequested_Returns499()
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
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _systemUnderTest.CreateBookingAsync(model, cts.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(499));
        }
    }
}
