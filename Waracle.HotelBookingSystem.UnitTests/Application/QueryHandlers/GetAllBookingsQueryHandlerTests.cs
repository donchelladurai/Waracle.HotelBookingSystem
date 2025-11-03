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
    public class GetAllBookingsQueryHandlerTests
    {
        private GetAllBookingsQueryHandler _systemUnderTest;
        private Mock<IBookingsRepository> _bookingsRepositoryMock;
        private Mock<ILogger<GetAllBookingsQueryHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _bookingsRepositoryMock = new Mock<IBookingsRepository>();
            _loggerMock = new Mock<ILogger<GetAllBookingsQueryHandler>>();
            _systemUnderTest = new GetAllBookingsQueryHandler(_bookingsRepositoryMock.Object, _loggerMock.Object);
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
            Assert.That(() => new GetAllBookingsQueryHandler(null, _loggerMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new GetAllBookingsQueryHandler(_bookingsRepositoryMock.Object, null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Handle_ReturnsAllBookings_WhenBookingsExist()
        {
            var request = new GetAllBookingsQuery();

            var bookings = new List<Booking>
            {
                new Booking
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
                }
            };

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().BookingReference, Is.EqualTo("REF123"));
            Assert.That(result.First().HotelName, Is.EqualTo("Hotel1"));
            Assert.That(result.First().RoomType, Is.EqualTo("Standard"));
            Assert.That(result.First().NumberOfGuests, Is.EqualTo(2));
        }

        [Test]
        public async Task Handle_ReturnsEmpty_WhenNoBookingsExist()
        {
            var request = new GetAllBookingsQuery();

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetAllBookingsQuery();

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }
}
