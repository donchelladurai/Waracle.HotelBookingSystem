using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Application.QueryHandlers;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.UnitTests.Application.QueryHandlers
{
    [TestFixture]
    public class GetBookingByReferenceQueryHandlerTests
    {
        private GetBookingByReferenceQueryHandler _systemUnderTest;
        private Mock<IBookingsRepository> _bookingsRepositoryMock;
        private Mock<ILogger<GetBookingByReferenceQueryHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _bookingsRepositoryMock = new Mock<IBookingsRepository>();
            _loggerMock = new Mock<ILogger<GetBookingByReferenceQueryHandler>>();
            _systemUnderTest = new GetBookingByReferenceQueryHandler(_bookingsRepositoryMock.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest = null;
            _bookingsRepositoryMock = null;
            _loggerMock = null;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenBookingsRepositoryIsNull()
        {
            Assert.That(() => new GetBookingByReferenceQueryHandler(null, _loggerMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new GetBookingByReferenceQueryHandler(_bookingsRepositoryMock.Object, null), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            Assert.That(() => _systemUnderTest.Handle(null, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenBookingReferenceIsNull()
        {
            var request = new GetBookingByReferenceQuery(null);
            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Handle_ReturnsBookingDto_WhenBookingExists()
        {
            var request = new GetBookingByReferenceQuery("REF123");

            var booking = new Booking
            {
                Reference = "REF123",
                Room = new Room
                {
                    HotelId = 1,
                    Id = 1,
                    Hotel = new Hotel { Name = "Hotel1" },
                    RoomType = new RoomType { Name = "Standard" }
                },
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(1),
                NumberOfGuests = 2
            };

            _bookingsRepositoryMock.Setup(r => r.FindByReferenceAsync("REF123", It.IsAny<CancellationToken>())).ReturnsAsync(booking);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BookingReference, Is.EqualTo("REF123"));
            Assert.That(result.HotelName, Is.EqualTo("Hotel1"));
            Assert.That(result.RoomType, Is.EqualTo("Standard"));
            Assert.That(result.NumberOfGuests, Is.EqualTo(2));
        }

        [Test]
        public async Task Handle_ReturnsNull_WhenBookingDoesNotExist()
        {
            var request = new GetBookingByReferenceQuery("REF123");

            _bookingsRepositoryMock.Setup(r => r.FindByReferenceAsync("REF123", It.IsAny<CancellationToken>())).ReturnsAsync((Booking)null);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var request = new GetBookingByReferenceQuery("REF123");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _systemUnderTest.Handle(request, cts.Token), Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetBookingByReferenceQuery("REF123");

            _bookingsRepositoryMock.Setup(r => r.FindByReferenceAsync("REF123", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }
}
